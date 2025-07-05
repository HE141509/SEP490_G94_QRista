using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Threading.Tasks;

namespace QRB.Pages.NguyenLieu
{
    public class AddNguyenLieuModel : PageModel
    {
        private readonly QRBDbContext _context;
        public AddNguyenLieuModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? TenNguyenLieu { get; set; }
        [BindProperty]
        public string? MaNguyenLieu { get; set; }
        [BindProperty]
        public string? DonViTinh { get; set; }

        public IActionResult OnGet()
        {
            // Không cho truy cập GET trực tiếp
            return RedirectToPage("/NguyenLieu/NguyenLieuList");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(TenNguyenLieu) || string.IsNullOrWhiteSpace(MaNguyenLieu) || string.IsNullOrWhiteSpace(DonViTinh))
            {
                TempData["AddNguyenLieuError"] = "Vui lòng nhập đầy đủ thông tin.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            // Sử dụng QRB.Models.NguyenLieu để tránh lỗi namespace
            if (_context.Set<QRB.Models.NguyenLieu>().Any(x => x.MaNguyenLieu == MaNguyenLieu && !x.IsDelete))
            {
                TempData["AddNguyenLieuError"] = "Mã nguyên liệu đã tồn tại.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            var nl = new QRB.Models.NguyenLieu
            {
                ID = Guid.NewGuid(),
                TenNguyenLieu = TenNguyenLieu.Trim(),
                MaNguyenLieu = MaNguyenLieu.Trim(),
                DonViTinh = DonViTinh.Trim(),
                CreateTime = DateTime.Now,
                IsDelete = false
            };
            _context.Add(nl);
            await _context.SaveChangesAsync();
            TempData["AddNguyenLieuSuccess"] = "Thêm nguyên liệu thành công!";
            return RedirectToPage("/NguyenLieu/NguyenLieuList");
        }
    }
}
