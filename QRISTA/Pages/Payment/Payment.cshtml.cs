using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using QRB.Helpers;
using QRB.Data;
using QRB.Models;
using System;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace QRB.Pages.Payment
{
    public class PaymentModel : PageModel
    {
        private readonly QRBDbContext _context;

        public PaymentModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Amount { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? CartData { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Table { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Phone { get; set; }
        
        public string? PaymentUrl { get; set; }
        public string? OrderId { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            try
            {
                if (Amount < 1000) 
                {
                    ErrorMessage = "Số tiền thanh toán phải lớn hơn 1,000 VND";
                    return;
                }

                string txnRef = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                OrderId = txnRef;

                var order = CreateOrder(txnRef, Amount);
                if (order == null)
                {
                    ErrorMessage = "Không thể tạo đơn hàng. Vui lòng thử lại.";
                    return;
                }

                CreateOrderDetails(order.ID, txnRef);

                string tmnCode = "PT8AZLP3";
                string hashSecret = "PWSHBSJCVDL54CGNA6C1F55ZZIPV6XP2";
                string orderInfo = $"Thanh toan don hang QRista Cafe - {txnRef}";
                string orderType = "billpayment";

                string returnUrl = $"http://localhost:5233/VnpayReturn?orderId={txnRef}";
                string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                PaymentUrl = VnpayHelper.CreateVnpayPaymentUrl(
                    tmnCode,
                    hashSecret,
                    Amount,
                    orderInfo,
                    orderType,
                    returnUrl,
                    clientIp,
                    txnRef
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
            }
        }

        private DonHang? CreateOrder(string orderCode, int totalAmount)
        {
            try
            {
                var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
                var tableNumber = HttpContext.Session.GetInt32("TableNumber");
                var branchId = HttpContext.Session.GetString("BranchCode");

                Guid? customerId = null;
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var customer = _context.KhachHangs.FirstOrDefault(kh => kh.SDT == phoneNumber);
                    customerId = customer?.ID;
                }
                Guid chiNhanhId = !string.IsNullOrEmpty(branchId) ? Guid.Parse(branchId) : Guid.Empty;

                
                if (chiNhanhId == Guid.Empty)
                {
                    var defaultBranch = _context.ChiNhanhs.FirstOrDefault();
                    chiNhanhId = defaultBranch?.ID ?? Guid.Empty;
                }

                var order = new DonHang
                {
                    ID = Guid.NewGuid(),
                    IDKhachHang = customerId,
                    IDChiNhanh = chiNhanhId,
                    MaDonHang = orderCode,
                    TongTien = totalAmount.ToString(),
                    TrangThaiThanhToan = false,
                    CreateTime = DateTime.Now,
                    IsDelete = false
                };

                _context.DonHangs.Add(order);
                _context.SaveChanges();

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex}");
                return null;
            }
        }

        private void CreateOrderDetails(Guid orderId, string orderCode)
        {
            try
            {
                var cartDataJson = HttpContext.Session.GetString("qrb_cart_data");
                if (string.IsNullOrEmpty(cartDataJson))
                {
                    return;
                }
                // Parse cart data (giả sử format: { "key": { "name": "...", "qty": 1, "price": 30000, "maLoai": "..." } })
                var cartData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cartDataJson);
                if (cartData == null || !cartData.Any())
                {
                    return;
                }

                foreach (var item in cartData)
                {
                    if (item.Value.qty <= 0) continue;

                    // Tìm sản phẩm trong database
                    var product = _context.SanPhams.FirstOrDefault(sp => sp.TenSanPham == item.Value.name);
                    if (product == null)
                    {
                        Console.WriteLine($"Product not found: {item.Value.name}");
                        continue;
                    }

                    // Tìm loại sản phẩm nếu có
                    Guid loaiSanPhamId = Guid.Empty;
                    if (!string.IsNullOrEmpty(item.Value.maLoai))
                    {
                        var loaiSanPham = _context.LoaiSanPhams.FirstOrDefault(lsp => 
                            lsp.IDSanPham == product.ID && lsp.MaLoai == item.Value.maLoai);
                        if (loaiSanPham != null)
                        {
                            loaiSanPhamId = loaiSanPham.ID;
                        }
                        else
                        {
                            // Nếu không tìm thấy loại sản phẩm, tạo loại mặc định
                            var defaultLoai = _context.LoaiSanPhams.FirstOrDefault(lsp => lsp.IDSanPham == product.ID);
                            loaiSanPhamId = defaultLoai?.ID ?? Guid.Empty;
                        }
                    }
                    else
                    {
                        // Lấy loại sản phẩm đầu tiên nếu không chỉ định
                        var defaultLoai = _context.LoaiSanPhams.FirstOrDefault(lsp => lsp.IDSanPham == product.ID);
                        loaiSanPhamId = defaultLoai?.ID ?? Guid.Empty;
                    }

                    if (loaiSanPhamId == Guid.Empty)
                    {
                        Console.WriteLine($"No product type found for product: {item.Value.name}");
                        continue;
                    }

                    // Tạo chi tiết đơn hàng
                    var orderDetail = new ChiTietDonHang
                    {
                        ID = Guid.NewGuid(),
                        IDDonHang = orderId,
                        IDSanPham = product.ID,
                        IDLoaiSanPham = loaiSanPhamId,
                        SoLuong = item.Value.qty,
                        DonGia = item.Value.price.ToString(),
                        ThanhTien = (item.Value.qty * item.Value.price).ToString(),
                        CreateTime = DateTime.Now,
                        IsDelete = false
                    };

                    _context.ChiTietDonHangs.Add(orderDetail);
                }

                _context.SaveChanges();
                Console.WriteLine($"Created order details for order: {orderCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order details: {ex}");
            }
        }
    }

    // Model cho cart item
    public class CartItem
    {
        public string name { get; set; } = "";
        public int qty { get; set; }
        public int price { get; set; }
        public string? maLoai { get; set; }
    }
}
