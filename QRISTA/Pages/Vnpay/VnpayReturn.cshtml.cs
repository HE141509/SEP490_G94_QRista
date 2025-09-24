using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using QRB.Helpers;
using QRB.Data;
using QRB.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrderModel = QRB.Models.Order;

namespace QRB.Pages.Vnpay
{
    public class VnpayReturnModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly IVnpaySecurityService _securityService;
        private readonly ILogger<VnpayReturnModel> _logger;

        public VnpayReturnModel(QRBDbContext context, IVnpaySecurityService securityService, ILogger<VnpayReturnModel> logger)
        {
            _context = context;
            _securityService = securityService;
            _logger = logger;
        }

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? OrderId { get; set; }
        // Lưu toàn bộ tham số trả về từ VNPAY (bao gồm cả các trường không phải vnp_)
        public Dictionary<string, string> AllParams { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> VnpParams { get; set; } = new Dictionary<string, string>();
        public void OnGet()
        {
            var query = HttpContext.Request.Query;
            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            // SECURITY: Validate request before processing
            if (!_securityService.ValidateReturnRequest(query.ToDictionary(q => q.Key, q => q.Value.ToString()), clientIp))
            {
                IsSuccess = false;
                Message = "Yêu cầu không hợp lệ hoặc bị từ chối!";
                _logger.LogWarning("Invalid VNPAY return request from IP: {ClientIP}", clientIp);
                return;
            }

            // Lưu tất cả tham số trả về (bao gồm cả các trường không phải vnp_)
            foreach (var key in query.Keys)
            {
                var value = query[key].ToString();
                if (!string.IsNullOrEmpty(value))
                    AllParams[key] = value;
                if (key.StartsWith("vnp_") && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    VnpParams[key] = value;
                }
            }

            // Lấy secure hash
            string? vnp_SecureHash = query.ContainsKey("vnp_SecureHash") ? query["vnp_SecureHash"].ToString() : null;

            // SECURITY: Check for replay attack
            string transactionRef = VnpParams.ContainsKey("vnp_TxnRef") ? VnpParams["vnp_TxnRef"] : "";
            if (!string.IsNullOrEmpty(transactionRef) && !_securityService.IsValidTransaction(transactionRef))
            {
                IsSuccess = false;
                Message = "Giao dịch đã được xử lý trước đó!";
                _logger.LogWarning("Replay attack detected for transaction: {TxnRef} from IP: {ClientIP}", transactionRef, clientIp);
                return;
            }

            // Sắp xếp params theo key và build chuỗi hash đúng chuẩn VNPAY
            var ordered = VnpParams.OrderBy(x => x.Key);
            StringBuilder data = new StringBuilder();
            foreach (var kv in ordered)
            {
                if (data.Length > 0) data.Append('&');
                data.Append(kv.Key + "=" + kv.Value);
            }

            // Lấy secret key
            string vnp_HashSecret = VnpayHelper.Vnp_HashSecret;
            string checkHash = VnpayHelper.HmacSHA512(vnp_HashSecret, data.ToString());

            // Kiểm tra checksum
            if (!string.IsNullOrEmpty(vnp_SecureHash) && vnp_SecureHash.Equals(checkHash, System.StringComparison.InvariantCultureIgnoreCase))
            {
                // SECURITY: Mark transaction as processed to prevent replay
                if (!string.IsNullOrEmpty(transactionRef))
                {
                    _securityService.MarkTransactionAsProcessed(transactionRef);
                }

                // Xác thực thành công
                if (VnpParams.ContainsKey("vnp_ResponseCode") && VnpParams["vnp_ResponseCode"] == "00")
                {
                    IsSuccess = true;
                    Message = "Thanh toán thành công!";

                    // Cập nhật trạng thái đơn hàng
                    if (query.ContainsKey("orderId"))
                    {
                        OrderId = query["orderId"].ToString();
                        try
                        {
                            var order = _context.Orders.FirstOrDefault(o => o.OrderCode == OrderId);
                            if (order != null)
                            {
                                // SECURITY: Double-check order hasn't been paid already
                                if (order.PaymentStatus == true)
                                {
                                    Message = $"Đơn hàng {OrderId} đã được thanh toán trước đó.";
                                    _logger.LogWarning("Attempt to pay already paid order: {OrderId} from IP: {ClientIP}", OrderId, clientIp);
                                    return;
                                }

                                order.PaymentStatus = true;
                                order.PaymentDate = DateTime.Now;
                                order.UpdateTime = DateTime.Now;
                                
                                // Cập nhật GiaTriDonHang cho khách hàng
                                if (order.IDCustomer.HasValue)
                                {
                                    var customer = _context.Customers.FirstOrDefault(c => c.ID == order.IDCustomer);
                                    if (customer != null)
                                    {
                                        decimal orderAmount = 0;
                                        if (decimal.TryParse(order.Amount, out orderAmount))
                                        {
                                            decimal currentGiaTri = 0;
                                            if (!string.IsNullOrWhiteSpace(customer.GiaTriDonHang))
                                            {
                                                decimal.TryParse(customer.GiaTriDonHang, out currentGiaTri);
                                            }
                                            customer.GiaTriDonHang = (currentGiaTri + orderAmount).ToString();
                                            customer.UpdateTime = DateTime.Now;
                                        }
                                    }
                                }
                                
                                // Lưu thông tin giao dịch VNPAY
                                if (VnpParams.ContainsKey("vnp_TransactionNo"))
                                {
                                    // Có thể lưu mã giao dịch VNPAY vào một trường riêng nếu cần
                                    _logger.LogInformation("VNPAY Transaction: {TransactionNo} for Order: {OrderId}", VnpParams["vnp_TransactionNo"], OrderId);
                                }
                                
                                _context.SaveChanges();
                                Message = $"Thanh toán thành công! Mã đơn hàng: {OrderId}";
                                _logger.LogInformation("Payment successful for order: {OrderId} from IP: {ClientIP}", OrderId, clientIp);
                            }
                            else
                            {
                                Message = $"Thanh toán thành công nhưng không tìm thấy đơn hàng {OrderId} trong hệ thống.";
                                _logger.LogWarning("Payment successful but order not found: {OrderId}", OrderId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error updating order: {OrderId}", OrderId);
                            Message = "Thanh toán thành công nhưng có lỗi khi cập nhật đơn hàng.";
                        }
                    }
                    else
                    {
                        Message = "Thanh toán thành công nhưng không có thông tin đơn hàng.";
                    }
                }
                else
                {
                    IsSuccess = false;
                    Message = $"Thanh toán thất bại. Mã lỗi: {VnpParams.GetValueOrDefault("vnp_ResponseCode", "unknown")}";
                    _logger.LogWarning("Payment failed with response code: {ResponseCode} for transaction: {TxnRef}", 
                        VnpParams.GetValueOrDefault("vnp_ResponseCode", "unknown"), transactionRef);
                }
            }
            else
            {
                IsSuccess = false;
                Message = "Sai chữ ký xác thực hoặc dữ liệu bị giả mạo!";
                _logger.LogWarning("Invalid signature detected from IP: {ClientIP} for transaction: {TxnRef}", clientIp, transactionRef);
            }

            // Lấy orderId nếu có
            if (query.ContainsKey("orderId"))
                OrderId = query["orderId"].ToString();
            else if (VnpParams.ContainsKey("vnp_TxnRef"))
                OrderId = VnpParams["vnp_TxnRef"];
        }
    }
}
