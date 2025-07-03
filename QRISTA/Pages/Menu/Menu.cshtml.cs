using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System.Collections.Generic;
using System.Linq;

namespace QRB.Pages.Menu
{
    public class MenuModel : PageModel
    {
        private readonly QRBDbContext _context;
        public bool IsLoggedIn { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string ChiNhanhName { get; set; } = string.Empty;
        public List<NhomSanPham> ProductGroups { get; set; } = new();

        public MenuModel(QRBDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            // Kiểm tra trạng thái đăng nhập
            IsLoggedIn = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
            if (IsLoggedIn)
            {
                DisplayName = HttpContext.Session.GetString("DisplayName") ?? "Người dùng";
                ChiNhanhName = HttpContext.Session.GetString("ChiNhanhName") ?? "Chi nhánh";
            }

            // Lấy danh sách nhóm sản phẩm từ DB (chỉ lấy nhóm chưa xóa)
            ProductGroups = _context.NhomSanPhams
                .Where(x => !x.IsDelete)
                .OrderBy(x => x.TenNhom)
                .ToList();
        }

        // API: Lấy sản phẩm và loại sản phẩm theo nhóm (Razor Pages: ?handler=GetProductsByGroup&maNhom=...)
        public JsonResult OnGetGetProductsByGroup(string maNhom)
        {
            // Nếu maNhom là "all" thì lấy tất cả sản phẩm chưa xóa
            var query = _context.SanPhams.Where(x => !x.IsDelete);
            if (!string.IsNullOrEmpty(maNhom) && maNhom != "all")
            {
                query = query.Where(x => x.NhomSanPham.MaNhom == maNhom);
            }

            // Lấy loại sản phẩm thuộc các sản phẩm này
            var productTypes = _context.LoaiSanPhams
                .Where(x => !x.IsDelete && query.Select(p => p.ID).Contains(x.IDSanPham))
                .Select(x => new
                {
                    x.ID,
                    x.MaLoai,
                    x.TenLoai,
                    x.IDSanPham,
                    // DonGiaRaw = Microsoft.EntityFrameworkCore.EF.Property<object>(x, "DonGia")
                })
                .AsEnumerable() // chuyển sang LINQ to Objects để xử lý logic C#
                .Select(x => new
                {
                    x.ID,
                    x.MaLoai,
                    x.TenLoai,
                    x.IDSanPham,
                    // DonGia = TryParseDonGia(x.DonGiaRaw)
                })
                .ToList();

            // Lọc ra các sản phẩm có ít nhất 1 loại sản phẩm
            var productIdsWithTypes = productTypes.Select(x => x.IDSanPham).Distinct().ToHashSet();
            var products = query.Where(x => productIdsWithTypes.Contains(x.ID)).Select(x => new
            {
                x.ID,
                x.MaSanPham,
                x.TenSanPham,
                x.HinhAnh,
                x.IdNhomSanPham,
                x.IDChiNhanh
            }).ToList();

            // Hàm parse DonGia an toàn
            static decimal? TryParseDonGia(object donGiaObj)
            {
                if (donGiaObj == null) return null;
                if (donGiaObj is decimal d) return d;
                if (donGiaObj is double db) return (decimal)db;
                if (donGiaObj is float f) return (decimal)f;
                if (donGiaObj is int i) return i;
                if (donGiaObj is long l) return l;
                var str = donGiaObj.ToString();
                if (string.IsNullOrWhiteSpace(str)) return null;
                // Loại bỏ dấu phẩy, dấu cách, .00, ký tự lạ
                str = str.Replace(",", "").Replace(".00", "").Replace(" ", "").Trim();
                // Giữ lại ký tự số
                str = new string(str.Where(char.IsDigit).ToArray());
                if (string.IsNullOrWhiteSpace(str)) return null;
                if (decimal.TryParse(str, out var result)) return result;
                return null;
            }

            return new JsonResult(new { products, productTypes });
        }
    }
}
