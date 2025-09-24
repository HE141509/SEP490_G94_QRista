using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderModel = QRB.Models.Order;

namespace QRB.Pages.Order
{
    public class CancelOrderModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly ILogger<CancelOrderModel> _logger;
        
        public CancelOrderModel(QRBDbContext context, ILogger<CancelOrderModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class CancelOrderRequest
        {
            public string? orderId { get; set; }  // Lowercase để khớp với JSON từ frontend
            public string? reason { get; set; }   // Lowercase để khớp với JSON từ frontend
            public string? requestType { get; set; } // Lowercase để khớp với JSON từ frontend
        }

        public class RefundApprovalRequest
        {
            public string? orderId { get; set; } // Lowercase để khớp với JSON từ JavaScript
            public string? refundReason { get; set; }
            public string? refundAmount { get; set; }
            public string? managerId { get; set; }
        }

        // Hủy đơn hàng chưa trả hàng
        public async Task<IActionResult> OnPostCancelAsync()
        {
            CancelOrderRequest? req = null;
            try
            {
                string body;
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }
                
                _logger.LogInformation("Cancel request body: {Body}", body);
                
                req = System.Text.Json.JsonSerializer.Deserialize<CancelOrderRequest>(body);
                
                _logger.LogInformation("Parsed request - OrderId: {OrderId}, Reason: {Reason}, RequestType: {RequestType}", 
                    req?.orderId, req?.reason, req?.requestType);
                
                if (req == null || string.IsNullOrEmpty(req.orderId))
                {
                    _logger.LogWarning("Cancel request failed - missing order information. Req is null: {IsNull}, OrderId: {OrderId}", 
                        req == null, req?.orderId);
                    return new JsonResult(new { success = false, message = "Thiếu thông tin đơn hàng." });
                }
                
                if (!Guid.TryParse(req.orderId, out Guid orderId))
                {
                    _logger.LogWarning("Cancel request failed - invalid order ID: {OrderId}", req.orderId);
                    return new JsonResult(new { success = false, message = "Mã đơn hàng không hợp lệ." });
                }

                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn hàng." });
                }

                // Kiểm tra đơn hàng đã bị hủy/xóa chưa
                if (order.IsDelete || order.IsCancelled == true)
                {
                    return new JsonResult(new { success = false, message = "Đơn hàng này đã được hủy hoặc xóa trước đó." });
                }

                // LOGIC MỚI: Kiểm tra trạng thái đơn hàng
                if (order.PaymentStatus == true)
                {
                    // Đơn đã thanh toán - cần quy trình hoàn tiền
                    if (order.Served == true)
                    {
                        return new JsonResult(new { 
                            success = false, 
                            message = "Không thể hủy đơn hàng đã trả hàng. Vui lòng liên hệ quản lý để xử lý hoàn tiền." 
                        });
                    }
                    else
                    {
                        return new JsonResult(new { 
                            success = false, 
                            message = "Đơn hàng đã thanh toán cần xác nhận quản lý để hoàn tiền.",
                            requiresManagerApproval = true,
                            orderId = req.orderId
                        });
                    }
                }

                // Đơn chưa thanh toán - cho phép hủy bình thường
                string userId = HttpContext.Session.GetString("UserId") ?? "";
                if (!Guid.TryParse(userId, out Guid cancelUserId))
                {
                    return new JsonResult(new { success = false, message = "Phiên đăng nhập không hợp lệ." });
                }

                // Cập nhật trạng thái hủy
                order.IsCancelled = true;
                order.CancelledDate = DateTime.Now;
                order.CancelReason = req.reason ?? "Hủy đơn hàng";
                order.CancelledByUserId = cancelUserId;
                order.UpdateTime = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully cancelled unpaid order {OrderId} by user {UserId}. Reason: {Reason}", 
                    orderId, cancelUserId, req.reason);

                return new JsonResult(new { 
                    success = true, 
                    message = "Hủy đơn hàng thành công!" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", req?.orderId);
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra khi hủy đơn hàng: " + ex.Message });
            }
        }

        // Duyệt hoàn tiền cho đơn đã thanh toán (chỉ quản lý)
        public async Task<IActionResult> OnPostApproveRefundAsync()
        {
            RefundApprovalRequest? req = null;
            try
            {
                string body;
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }
                req = System.Text.Json.JsonSerializer.Deserialize<RefundApprovalRequest>(body);
                
                if (req == null || string.IsNullOrEmpty(req.orderId))
                {
                    return new JsonResult(new { success = false, message = "Thiếu thông tin đơn hàng." });
                }

                // Kiểm tra quyền quản lý
                string userRole = HttpContext.Session.GetString("VaiTro") ?? "";
                if (userRole != "QuanLy" && userRole != "Admin")
                {
                    return new JsonResult(new { success = false, message = "Chỉ quản lý mới có quyền duyệt hoàn tiền." });
                }

                if (!Guid.TryParse(req.orderId, out Guid orderId))
                {
                    return new JsonResult(new { success = false, message = "Mã đơn hàng không hợp lệ." });
                }

                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn hàng." });
                }

                // Kiểm tra điều kiện hoàn tiền
                if (order.PaymentStatus != true)
                {
                    return new JsonResult(new { success = false, message = "Chỉ có thể hoàn tiền cho đơn hàng đã thanh toán." });
                }

                if (order.IsRefunded == true)
                {
                    return new JsonResult(new { success = false, message = "Đơn hàng này đã được hoàn tiền trước đó." });
                }

                string managerId = HttpContext.Session.GetString("UserId") ?? "";
                if (!Guid.TryParse(managerId, out Guid managerGuid))
                {
                    return new JsonResult(new { success = false, message = "Phiên đăng nhập không hợp lệ." });
                }

                // Cập nhật trạng thái hoàn tiền
                order.IsRefunded = true;
                order.RefundDate = DateTime.Now;
                order.RefundReason = req.refundReason ?? "Hoàn tiền theo yêu cầu";
                order.RefundApprovedByUserId = managerGuid;
                order.RefundAmount = req.refundAmount ?? order.Amount;
                order.IsCancelled = true;
                order.CancelledDate = DateTime.Now;
                order.CancelReason = "Hoàn tiền - " + (req.refundReason ?? "Theo yêu cầu");
                order.UpdateTime = DateTime.Now;

                // Trừ tiền từ giá trị đơn hàng của khách hàng
                if (order.IDCustomer.HasValue)
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ID == order.IDCustomer);
                    if (customer != null)
                    {
                        decimal refundAmount = 0;
                        if (decimal.TryParse(order.RefundAmount, out refundAmount))
                        {
                            decimal currentGiaTri = 0;
                            if (!string.IsNullOrWhiteSpace(customer.GiaTriDonHang))
                            {
                                decimal.TryParse(customer.GiaTriDonHang, out currentGiaTri);
                            }
                            
                            // Trừ số tiền hoàn lại khỏi giá trị đơn hàng của khách hàng
                            decimal newGiaTri = currentGiaTri - refundAmount;
                            if (newGiaTri < 0) newGiaTri = 0; // Không cho phép giá trị âm
                            
                            customer.GiaTriDonHang = newGiaTri.ToString();
                            customer.UpdateTime = DateTime.Now;
                            
                            _logger.LogInformation("Customer {CustomerId} order value reduced by {RefundAmount}. Old value: {OldValue}, New value: {NewValue}", 
                                customer.ID, refundAmount, currentGiaTri, newGiaTri);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Refund approved for order {OrderId} by manager {ManagerId}. Amount: {RefundAmount}", 
                    orderId, managerGuid, req.refundAmount);

                return new JsonResult(new { 
                    success = true, 
                    message = "Duyệt hoàn tiền thành công!" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving refund for order {OrderId}", req?.orderId);
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra khi duyệt hoàn tiền: " + ex.Message });
            }
        }
    }
}
