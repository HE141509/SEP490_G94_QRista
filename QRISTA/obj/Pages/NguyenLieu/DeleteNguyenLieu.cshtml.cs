using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Threading.Tasks;

namespace QRB.Pages.NguyenLieu
{
    public class DeleteNguyenLieuModel : PageModel
    {
        private readonly QRBDbContext _context;
        public DeleteNguyenLieuModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Guid NguyenLieuId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public QRB.Models.NguyenLieu? NguyenLieu { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                ErrorMessage = "Không tìm thấy nguyên liệu.";
                return Page();
            }
            NguyenLieu = await _context.NguyenLieus.FindAsync(id);
            if (NguyenLieu == null)
            {
                ErrorMessage = "Không tìm thấy nguyên liệu.";
                return Page();
            }
            NguyenLieuId = NguyenLieu.ID;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var nguyenLieu = await _context.NguyenLieus.FindAsync(NguyenLieuId);
            if (nguyenLieu == null)
            {
                ErrorMessage = "Không tìm thấy nguyên liệu.";
                return Page();
            }
            nguyenLieu.IsDelete = true;
            await _context.SaveChangesAsync();
            SuccessMessage = "Xóa nguyên liệu thành công.";
            return RedirectToPage("NguyenLieuList");
        }
    }
}
