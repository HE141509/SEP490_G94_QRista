using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using QRB.Services;
using System.Threading.Tasks;

namespace QRB.Pages.NguyenLieu
{
    public class DeleteNguyenLieuModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly IIngredientService _ingredientService;
        
        public DeleteNguyenLieuModel(QRBDbContext context, IIngredientService ingredientService)
        {
            _context = context;
            _ingredientService = ingredientService;
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
            NguyenLieu = await _ingredientService.GetIngredientByIdAsNguyenLieuAsync(id.Value);
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
            var success = await _ingredientService.DeleteIngredientAsync(NguyenLieuId);
            if (!success)
            {
                ErrorMessage = "Không tìm thấy nguyên liệu.";
                return Page();
            }
            SuccessMessage = "Xóa nguyên liệu thành công.";
            return RedirectToPage("NguyenLieuList");
        }
    }
}
