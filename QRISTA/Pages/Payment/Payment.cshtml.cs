using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using QRB.Helpers;
using QRB.Data;
using QRB.Models;
using System;
using System.Net;
using Microsoft.EntityFrameworkCore;
using OrderModel = QRB.Models.Order;

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
                // Debug: Check cart data when entering Payment page
                var cartDataFromSession = HttpContext.Session.GetString("qrb_cart_data");
                Console.WriteLine($"Payment OnGet - Cart data from session: {cartDataFromSession ?? "NULL"}");
                
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

        private OrderModel? CreateOrder(string orderCode, int totalAmount)
        {
            try
            {
                var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
                var tableNumber = HttpContext.Session.GetInt32("TableNumber");
                var branchId = HttpContext.Session.GetString("ChiNhanhId"); // Sửa từ BranchCode thành ChiNhanhId
                var userId = HttpContext.Session.GetString("UserId");
                
                Console.WriteLine($"CreateOrder - BranchId from session: {branchId ?? "NULL"}");
                Console.WriteLine($"CreateOrder - UserId from session: {userId ?? "NULL"}");

                Guid? customerId = null;
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var customer = _context.Customers.FirstOrDefault(c => c.Phone == phoneNumber && !c.IsDelete);
                    customerId = customer?.ID;
                }
                
                Guid chiNhanhId = !string.IsNullOrEmpty(branchId) ? Guid.Parse(branchId) : Guid.Empty;
                if (chiNhanhId == Guid.Empty)
                {
                    var defaultBranch = _context.Departments.FirstOrDefault();
                    chiNhanhId = defaultBranch?.ID ?? Guid.Empty;
                    Console.WriteLine($"Using default branch: {chiNhanhId}");
                }
                else
                {
                    Console.WriteLine($"Using branch from session: {chiNhanhId}");
                }

                // Lấy ID nhân viên từ session hoặc lấy nhân viên đầu tiên
                Guid nhanVienId = Guid.Empty;
                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid userGuid))
                {
                    nhanVienId = userGuid;
                }
                else
                {
                    // Nếu không có user trong session, lấy nhân viên đầu tiên của chi nhánh
                    var defaultUser = _context.NguoiDungs.FirstOrDefault(u => u.IDChiNhanh == chiNhanhId && !u.IsDelete);
                    nhanVienId = defaultUser?.ID ?? Guid.Empty;
                }

                var order = new OrderModel
                {
                    ID = Guid.NewGuid(),
                    IDCustomer = customerId,
                    IDEmployee = nhanVienId,
                    IDDepartment = chiNhanhId,
                    OrderCode = orderCode,
                    Amount = totalAmount.ToString(),
                    Price = totalAmount.ToString(),
                    PaymentStatus = false,
                    PaymentMethod = "Chuyển khoản",
                    CreateTime = DateTime.Now,
                    IsDelete = false,
                    Table = tableNumber
                };

                _context.Orders.Add(order);
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
                Console.WriteLine($"Cart data from session: {cartDataJson ?? "NULL"}");
                
                if (string.IsNullOrEmpty(cartDataJson))
                {
                    Console.WriteLine("Cart data is empty, cannot create order details");
                    return;
                }
                // Parse cart data (giả sử format: { "key": { "name": "...", "qty": 1, "price": 30000, "maLoai": "..." } })
                var cartData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cartDataJson);
                Console.WriteLine($"Parsed cart data items count: {cartData?.Count ?? 0}");
                
                if (cartData == null || !cartData.Any())
                {
                    Console.WriteLine("Cart data is null or empty after parsing");
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
                        Console.WriteLine($"No product type found for product: {item.Value.name}, creating with default type");
                        
                        // Tạo một loại sản phẩm mặc định nếu không tìm thấy
                        var defaultLoaiSanPham = new LoaiSanPham
                        {
                            ID = Guid.NewGuid(),
                            IDSanPham = product.ID,
                            TenLoai = "Mặc định",
                            MaLoai = "DEFAULT",
                            DonGia = item.Value.price.ToString(),
                            CreateTime = DateTime.Now,
                            IsDelete = false,
                            IDChiNhanh = product.IDChiNhanh
                        };
                        
                        _context.LoaiSanPhams.Add(defaultLoaiSanPham);
                        _context.SaveChanges();
                        loaiSanPhamId = defaultLoaiSanPham.ID;
                    }

                    // Tạo chi tiết đơn hàng
                    var orderDetail = new OrderDetail
                    {
                        ID = Guid.NewGuid(),
                        IDOrder = orderId,
                        IDProduct = product.ID,
                        IDProductType = loaiSanPhamId,
                        Quantity = item.Value.qty,
                        Price = item.Value.price.ToString(),
                        Total = (item.Value.qty * item.Value.price).ToString(),
                        CreateTime = DateTime.Now,
                        IsDelete = false
                    };

                    _context.OrderDetails.Add(orderDetail);
                }

                _context.SaveChanges();
                Console.WriteLine($"Created order details for order: {orderCode}");
                
                // Chỉ xóa cart data sau khi đã tạo xong order details thành công
                HttpContext.Session.Remove("qrb_cart_data");
                Console.WriteLine("Cart data cleared after successful order creation");
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
