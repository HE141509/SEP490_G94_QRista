
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
    }
}
