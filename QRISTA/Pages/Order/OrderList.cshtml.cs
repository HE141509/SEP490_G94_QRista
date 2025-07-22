
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace QRB.Pages.Order
{
    public class OrderListModel : PageModel
    {
        private readonly QRBDbContext _context;
        public List<DonHang> Orders { get; set; } = new();

        public OrderListModel(QRBDbContext context)
        {
            _context = context;
        }

        public string CurrentBranchName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        // API tìm khách hàng theo SĐT cho AJAX
        public JsonResult OnGetFindByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return new JsonResult(new { });
            var kh = _context.KhachHangs.FirstOrDefault(x => x.SDT == phone && !x.IsDelete);
            if (kh == null)
                return new JsonResult(new { });

            // Lấy danh sách mã ưu đãi thực từ DB
            var uuDais = _context.MaUuDais
                .Where(x => x.IDKhachHang == kh.ID && !x.IsDelete && !x.TrangThaiSuDung)
                .Select(x => new { x.MaGiamGia, x.TienGiam })
                .ToList();

            var maUuDaiList = uuDais.Select(x => x.MaGiamGia).ToList();
            string? maUuDaiMacDinh = maUuDaiList.FirstOrDefault();
            var tienGiamDict = uuDais.ToDictionary(x => x.MaGiamGia, x => x.TienGiam);

            return new JsonResult(new { id = kh.ID, name = kh.TenKhachHang, maUuDaiList, maUuDaiMacDinh, tienGiamDict });
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

            Orders = _context.DonHangs
                .Where(x => !x.IsDelete)
                .OrderByDescending(x => x.CreateTime)
                .Take(100)
                .Include(x => x.KhachHang)
                .Include(x => x.ChiNhanh)
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
                    TenNhomSanPham = x.NhomSanPham != null ? x.NhomSanPham.TenNhom : null
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

                // Lấy thông tin hóa đơn
                var order = await _context.DonHangs
                    .Include(d => d.KhachHang)
                    .Include(d => d.NhanVien)
                    .Include(d => d.ChiNhanh)
                    .FirstOrDefaultAsync(d => d.ID == id && !d.IsDelete);

                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                // Lấy chi tiết hóa đơn
                var orderDetails = await (from ct in _context.ChiTietDonHangs
                                         join sp in _context.SanPhams on ct.IDSanPham equals sp.ID
                                         join lsp in _context.LoaiSanPhams on ct.IDLoaiSanPham equals lsp.ID
                                         where ct.IDDonHang == id && !ct.IsDelete && !sp.IsDelete && !lsp.IsDelete
                                         select new
                                         {
                                             id = ct.ID,
                                             idSanPham = sp.ID,
                                             idLoaiSanPham = lsp.ID,
                                             tenSanPham = sp.TenSanPham,
                                             tenLoaiSanPham = lsp.TenLoai,
                                             donGia = lsp.DonGia,
                                             soLuong = ct.SoLuong,
                                             thanhTien = ct.ThanhTien
                                         })
                                         .ToListAsync();

                var result = new
                {
                    success = true,
                    data = new
                    {
                        id = order.ID,
                        maDonHang = order.MaDonHang,
                        idKhachHang = order.IDKhachHang,
                        tenKhachHang = order.KhachHang?.TenKhachHang ?? "",
                        sdtKhachHang = order.KhachHang?.SDT ?? "",
                        idNhanVien = order.IDNhanVien,
                        tenNhanVien = order.NhanVien?.TenHienThi ?? "",
                        idChiNhanh = order.IDChiNhanh,
                        tenChiNhanh = order.ChiNhanh?.TenChiNhanh ?? "",
                        maUuDai = order.MaUuDai ?? "",
                        tienUuDai = order.TienUuDai ?? "0",
                        tongTien = order.TongTien,
                        trangThaiThanhToan = order.TrangThaiThanhToan,
                        createTime = order.CreateTime.ToString("dd/MM/yyyy HH:mm"),
                        soBan = order.SoBan,
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

                // Tìm hóa đơn
                var order = await _context.DonHangs.FirstOrDefaultAsync(d => d.ID == orderId && !d.IsDelete);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                // Cập nhật thông tin hóa đơn
                if (!string.IsNullOrEmpty(data.MaUuDai))
                    order.MaUuDai = data.MaUuDai;
                if (!string.IsNullOrEmpty(data.TienUuDai))
                    order.TienUuDai = data.TienUuDai;
                if (!string.IsNullOrEmpty(data.TongTien))
                    order.TongTien = data.TongTien;
                
                order.TrangThaiThanhToan = data.TrangThaiThanhToan;
                order.UpdateTime = DateTime.Now;

                if (data.TrangThaiThanhToan && !order.NgayThanhToan.HasValue)
                {
                    order.NgayThanhToan = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Cập nhật hóa đơn thành công" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi cập nhật hóa đơn: " + ex.Message });
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
    }
}
