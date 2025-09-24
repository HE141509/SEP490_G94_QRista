
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using QRB.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using OrderModel = QRB.Models.Order;

namespace QRB.Pages.Order
{
    public class OrderListModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly IVoucherService _voucherService;
        public List<OrderModel> Orders { get; set; } = new();

        public OrderListModel(QRBDbContext context, IVoucherService voucherService)
        {
            _context = context;
            _voucherService = voucherService;
        }
        private bool HasPermission(string permissionName)
        {
            var permissionsJson = HttpContext.Session.GetString("UserPermissions");
            if (string.IsNullOrEmpty(permissionsJson))
            {
                return false;
            }
            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
                return permissions?.Contains(permissionName) ?? false;
            }
            catch
            {
                return false;
            }
        }

        public string CurrentBranchName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        // API tìm khách hàng theo SĐT cho AJAX
        public async Task<JsonResult> OnGetFindByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return new JsonResult(new { });
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Phone == phone && !x.IsDelete);
            if (customer == null)
                return new JsonResult(new { });

            // Lấy danh sách mã ưu đãi thực từ Voucher service
            var maUuDais = await _voucherService.GetVouchersByCustomerIdAsMaUuDaiAsync(customer.ID);
            var activeUuDais = maUuDais
                .Where(x => !x.IsDelete && !x.TrangThaiSuDung)
                .Select(x => new { x.MaGiamGia, x.TienGiam })
                .ToList();

            var maUuDaiList = activeUuDais.Select(x => x.MaGiamGia).ToList();
            string? maUuDaiMacDinh = maUuDaiList.FirstOrDefault();
            var tienGiamDict = activeUuDais.ToDictionary(x => x.MaGiamGia, x => x.TienGiam);

            return new JsonResult(new { id = customer.ID, name = customer.CustomerName, maUuDaiList, maUuDaiMacDinh, tienGiamDict });
        }

        public IActionResult OnGet()
        {
            // Kiểm tra đăng nhập - bắt buộc phải đăng nhập mới được truy cập
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                // Chưa đăng nhập, redirect về trang login
                return RedirectToPage("/Login");
            }
            if (!HasPermission("Full Invoices"))
            {
                return Redirect($"/AccessDenied?permission=Full Invoices&module=Invoices");
            }

            // Lấy chi nhánh của user đang đăng nhập
            var branchIdString = HttpContext.Session.GetString("ChiNhanhId");
            Guid? userBranchId = null;
            if (!string.IsNullOrEmpty(branchIdString) && Guid.TryParse(branchIdString, out Guid branchGuid))
            {
                userBranchId = branchGuid;
            }

            var ordersQuery = _context.Orders
                .Where(x => !x.IsDelete && (x.IsCancelled != true || x.IsRefunded == true)); // Hiển thị đơn bình thường và đơn hủy để hoàn tiền

            // Nếu có chi nhánh cụ thể, lọc theo chi nhánh đó
            if (userBranchId.HasValue)
            {
                ordersQuery = ordersQuery.Where(x => x.IDDepartment == userBranchId.Value);
            }

            Orders = ordersQuery
                .OrderByDescending(x => x.CreateTime)
                .Take(100)
                .Include(x => x.Customer)
                .Include(x => x.Department)
                .ToList();

            // Lấy thông tin chi nhánh từ session
            var branchName = HttpContext.Session.GetString("ChiNhanhName");
            if (!string.IsNullOrEmpty(branchName))
            {
                CurrentBranchName = branchName;
            }
            else
            {
                CurrentBranchName = "Chi nhánh mặc định";
            }

            var displayName = HttpContext.Session.GetString("DisplayName");
            if (!string.IsNullOrEmpty(displayName))
            {
                DisplayName = displayName;
            }
            else
            {
                DisplayName = "";
            }

            return Page();
        }
        // API tìm kiếm sản phẩm cho AJAX
        public JsonResult OnGetSearchProduct(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new JsonResult(new List<object>());
            var products = _context.SanPhams
                .Where(x => !x.IsDelete && x.TenSanPham.Contains(keyword))
                .Select(x => new {
                    ID = x.ID,
                    TenSanPham = x.TenSanPham,
                    TenNhomSanPham = x.Category != null ? x.Category.CategoryName : null
                })
                .Take(20)
                .ToList();
            return new JsonResult(products);
        }
        // API lấy danh sách loại sản phẩm theo ID sản phẩm
        public JsonResult OnGetGetProductTypes(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return new JsonResult(new List<object>());
            if (!Guid.TryParse(productId, out Guid sanPhamId))
                return new JsonResult(new List<object>());
            var types = _context.LoaiSanPhams
                .Where(x => !x.IsDelete && x.IDSanPham == sanPhamId)
                .Select(x => new {
                    ID = x.ID,
                    TenLoai = x.TenLoai,
                    DonGia = x.DonGia
                })
                .ToList();
            return new JsonResult(types);
        }

        // API lấy chi tiết loại sản phẩm theo ID loại sản phẩm
        public JsonResult OnGetGetProductTypeDetail(string typeId)
        {
            if (string.IsNullOrWhiteSpace(typeId))
                return new JsonResult(new { });
            if (!Guid.TryParse(typeId, out Guid loaiId))
                return new JsonResult(new { });
            var type = _context.LoaiSanPhams
                .Where(x => !x.IsDelete && x.ID == loaiId)
                .Select(x => new {
                    ID = x.ID,
                    TenLoai = x.TenLoai,
                    DonGia = x.DonGia
                })
                .FirstOrDefault();
            if (type != null)
                return new JsonResult(type);
            else
                return new JsonResult(new { });
        }

        // API lấy chi tiết hóa đơn
        public async Task<JsonResult> OnGetGetOrderDetailAsync(Guid id)
        {
            try
            {
                // Kiểm tra đăng nhập
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new { success = false, message = "Chưa đăng nhập" });
                }

                // Lấy chi nhánh của user đang đăng nhập
                var branchIdString = HttpContext.Session.GetString("ChiNhanhId");
                Guid? userBranchId = null;
                if (!string.IsNullOrEmpty(branchIdString) && Guid.TryParse(branchIdString, out Guid branchGuid))
                {
                    userBranchId = branchGuid;
                }

                // Lấy thông tin hóa đơn với kiểm tra chi nhánh
                var order = await _context.Orders
                    .Include(d => d.Customer)
                    .Include(d => d.Department)
                    .Where(d => d.ID == id && !d.IsDelete && 
                               (!userBranchId.HasValue || d.IDDepartment == userBranchId.Value))
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                // Lấy thông tin nhân viên nếu có
                string tenNhanVien = "N/A";
                if (order.IDEmployee != null && order.IDEmployee != Guid.Empty)
                {
                    var nhanVien = await _context.NguoiDungs
                        .Where(n => n.ID == order.IDEmployee)
                        .FirstOrDefaultAsync();
                    tenNhanVien = nhanVien?.TenHienThi ?? "N/A";
                }

                // Lấy chi tiết hóa đơn
                var orderDetails = await (from ct in _context.OrderDetails
                                         join sp in _context.SanPhams on ct.IDProduct equals sp.ID
                                         join lsp in _context.LoaiSanPhams on ct.IDProductType equals lsp.ID
                                         where ct.IDOrder == id && !ct.IsDelete && !sp.IsDelete && !lsp.IsDelete
                                         select new
                                         {
                                             id = ct.ID,
                                             idSanPham = sp.ID,
                                             idLoaiSanPham = lsp.ID,
                                             tenSanPham = sp.TenSanPham,
                                             tenLoaiSanPham = lsp.TenLoai,
                                             donGia = lsp.DonGia,
                                             soLuong = ct.Quantity,
                                             thanhTien = ct.Total
                                         })
                                         .ToListAsync();

                var result = new
                {
                    success = true,
                    data = new
                    {
                        id = order.ID,
                        maDonHang = order.OrderCode,
                        idKhachHang = order.IDCustomer,
                        tenKhachHang = order.Customer?.CustomerName ?? "",
                        sdtKhachHang = order.Customer?.Phone ?? "",
                        idNhanVien = order.IDEmployee,
                        tenNhanVien = tenNhanVien,
                        idChiNhanh = order.IDDepartment,
                        tenChiNhanh = order.Department?.DepartmentName ?? "",
                        maUuDai = order.VoucherCode ?? "",
                        tienUuDai = order.VoucherPrice ?? "0",
                        tongTien = order.Amount,
                        trangThaiThanhToan = order.PaymentStatus,
                        phuongThucThanhToan = order.PaymentMethod ?? "Tiền mặt",
                        createTime = order.CreateTime.ToString("dd/MM/yyyy HH:mm"),
                        soBan = order.Table,
                        chiTiet = orderDetails
                    }
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi lấy chi tiết hóa đơn: " + ex.Message });
            }
        }

        // API cập nhật hóa đơn
        public async Task<JsonResult> OnPostUpdateOrderAsync()
        {
            try
            {
                // Kiểm tra đăng nhập
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new { success = false, message = "Chưa đăng nhập" });
                }

                // Lấy chi nhánh của user đang đăng nhập
                var branchIdString = HttpContext.Session.GetString("ChiNhanhId");
                Guid? userBranchId = null;
                if (!string.IsNullOrEmpty(branchIdString) && Guid.TryParse(branchIdString, out Guid branchGuid))
                {
                    userBranchId = branchGuid;
                }

                // Đọc dữ liệu từ body
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                var data = System.Text.Json.JsonSerializer.Deserialize<OrderUpdateRequest>(body);
                if (data == null || !Guid.TryParse(data.ID, out Guid orderId))
                {
                    return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Tìm hóa đơn với kiểm tra chi nhánh
                var orderQuery = _context.Orders.Where(d => d.ID == orderId && !d.IsDelete);
                
                // Nếu có chi nhánh cụ thể, chỉ cho phép cập nhật đơn hàng của chi nhánh đó
                if (userBranchId.HasValue)
                {
                    orderQuery = orderQuery.Where(d => d.IDDepartment == userBranchId.Value);
                }

                var order = await orderQuery.FirstOrDefaultAsync();
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy hóa đơn hoặc không có quyền truy cập" });
                }

                // Cập nhật thông tin hóa đơn
                if (!string.IsNullOrEmpty(data.MaUuDai))
                    order.VoucherCode = data.MaUuDai;
                if (!string.IsNullOrEmpty(data.TienUuDai))
                    order.VoucherPrice = data.TienUuDai;
                if (!string.IsNullOrEmpty(data.TongTien))
                    order.Amount = data.TongTien;
                
                order.PaymentStatus = data.TrangThaiThanhToan;
                order.UpdateTime = DateTime.Now;

                if (data.TrangThaiThanhToan && !order.PaymentDate.HasValue)
                {
                    order.PaymentDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Cập nhật hóa đơn thành công" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi cập nhật hóa đơn: " + ex.Message });
            }
        }

        // Cập nhật trạng thái giao hàng
        public async Task<IActionResult> OnPostUpdateDeliveryStatus()
        {
            try
            {
                // Kiểm tra đăng nhập
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return new JsonResult(new { success = false, message = "Chưa đăng nhập" });
                }

                // Lấy chi nhánh của user đang đăng nhập
                var branchIdString = HttpContext.Session.GetString("ChiNhanhId");
                Guid? userBranchId = null;
                if (!string.IsNullOrEmpty(branchIdString) && Guid.TryParse(branchIdString, out Guid branchGuid))
                {
                    userBranchId = branchGuid;
                }

                var json = await new StreamReader(Request.Body).ReadToEndAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<UpdateDeliveryStatusRequest>(json);
                
                if (data == null || string.IsNullOrEmpty(data.OrderId) || !Guid.TryParse(data.OrderId, out Guid orderId))
                {
                    return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Tìm đơn hàng với kiểm tra chi nhánh
                var orderQuery = _context.Orders.Where(o => o.ID == orderId && !o.IsDelete);
                
                // Nếu có chi nhánh cụ thể, chỉ cho phép cập nhật đơn hàng của chi nhánh đó
                if (userBranchId.HasValue)
                {
                    orderQuery = orderQuery.Where(o => o.IDDepartment == userBranchId.Value);
                }

                var order = await orderQuery.FirstOrDefaultAsync();
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đơn hàng hoặc không có quyền truy cập" });
                }

                // Chỉ cho phép cập nhật nếu đơn hàng đã thanh toán
                if (order.PaymentStatus != true)
                {
                    return new JsonResult(new { success = false, message = "Chỉ có thể cập nhật trạng thái giao hàng cho đơn hàng đã thanh toán" });
                }

                order.Served = data.Delivered;
                order.UpdateTime = DateTime.Now;
                
                // Cập nhật ServedTime khi đơn hàng được đánh dấu là đã trả hàng
                if (data.Delivered && !order.ServedTime.HasValue)
                {
                    order.ServedTime = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Cập nhật trạng thái giao hàng thành công" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi cập nhật trạng thái giao hàng: " + ex.Message });
            }
        }

        // API loại bỏ hóa đơn nháp (chưa thanh toán)
        public async Task<JsonResult> OnPostCleanupDraftOrdersAsync()
        {
            try
            {
                // Sử dụng quyền order-management hoặc full-invoices thay vì Order.Delete
                if (!HasPermission("order-management") && !HasPermission("Full Invoices"))
                {
                    return new JsonResult(new { success = false, message = "Bạn không có quyền loại bỏ hóa đơn" });
                }

                var userIdString = HttpContext.Session.GetString("UserId");
                var branchIdString = HttpContext.Session.GetString("ChiNhanhId");

                if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(branchIdString))
                {
                    return new JsonResult(new { success = false, message = "Phiên đăng nhập không hợp lệ" });
                }

                if (!Guid.TryParse(branchIdString, out var userBranchId))
                {
                    return new JsonResult(new { success = false, message = "Thông tin chi nhánh không hợp lệ" });
                }

                // Tìm tất cả đơn hàng chưa thanh toán trong chi nhánh hiện tại
                var draftOrders = await _context.Orders
                    .Where(d => d.PaymentStatus != true && !d.IsDelete && d.IDDepartment == userBranchId)
                    .ToListAsync();

                if (!draftOrders.Any())
                {
                    return new JsonResult(new { success = true, deletedCount = 0, message = "Không có hóa đơn nháp nào để loại bỏ" });
                }

                // Lấy danh sách ID đơn hàng để xóa chi tiết
                var orderIds = draftOrders.Select(d => d.ID).ToList();

                // Xóa chi tiết đơn hàng trước
                var orderDetails = await _context.OrderDetails
                    .Where(ct => orderIds.Contains(ct.IDOrder))
                    .ToListAsync();

                _context.OrderDetails.RemoveRange(orderDetails);

                // Xóa đơn hàng
                _context.Orders.RemoveRange(draftOrders);

                await _context.SaveChangesAsync();

                return new JsonResult(new { 
                    success = true, 
                    deletedCount = draftOrders.Count, 
                    message = $"Đã loại bỏ thành công {draftOrders.Count} hóa đơn nháp" 
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { 
                    success = false, 
                    message = "Có lỗi khi loại bỏ hóa đơn nháp: " + ex.Message 
                });
            }
        }

        public class OrderUpdateRequest
        {
            public string ID { get; set; } = string.Empty;
            public string MaUuDai { get; set; } = string.Empty;
            public string TienUuDai { get; set; } = string.Empty;
            public string TongTien { get; set; } = string.Empty;
            public bool TrangThaiThanhToan { get; set; }
        }

        public class UpdateDeliveryStatusRequest
        {
            public string OrderId { get; set; } = string.Empty;
            public bool Delivered { get; set; }
        }
    }
}
