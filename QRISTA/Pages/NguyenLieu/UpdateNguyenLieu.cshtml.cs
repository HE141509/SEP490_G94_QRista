using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Threading.Tasks;

namespace QRB.Pages.NguyenLieu
{
    public class UpdateNguyenLieuModel : PageModel
    {
        private readonly QRBDbContext _context;
        public UpdateNguyenLieuModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Guid ID { get; set; }
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
            if (ID == Guid.Empty || string.IsNullOrWhiteSpace(TenNguyenLieu) || string.IsNullOrWhiteSpace(MaNguyenLieu) || string.IsNullOrWhiteSpace(DonViTinh))
            {
                TempData["UpdateNguyenLieuError"] = "Vui lòng nhập đầy đủ thông tin.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            var nl = await _context.Set<QRB.Models.NguyenLieu>().FindAsync(ID);
            if (nl == null || nl.IsDelete)
            {
                TempData["UpdateNguyenLieuError"] = "Không tìm thấy nguyên liệu hoặc đã bị xóa.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            // Kiểm tra trùng mã (trừ chính nó)
            if (_context.Set<QRB.Models.NguyenLieu>().Any(x => x.MaNguyenLieu == MaNguyenLieu && x.ID != ID && !x.IsDelete))
            {
                TempData["UpdateNguyenLieuError"] = "Mã nguyên liệu đã tồn tại.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            nl.TenNguyenLieu = TenNguyenLieu.Trim();
            nl.MaNguyenLieu = MaNguyenLieu.Trim();
            nl.DonViTinh = DonViTinh.Trim();
            nl.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["UpdateNguyenLieuSuccess"] = "Cập nhật nguyên liệu thành công!";
            return RedirectToPage("/NguyenLieu/NguyenLieuList");
        }
    }
}
