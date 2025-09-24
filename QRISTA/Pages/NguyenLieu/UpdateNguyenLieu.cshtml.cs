using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using QRB.Services;
using System.Threading.Tasks;

namespace QRB.Pages.NguyenLieu
{
    public class UpdateNguyenLieuModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly IIngredientService _ingredientService;
        
        public UpdateNguyenLieuModel(QRBDbContext context, IIngredientService ingredientService)
        {
            _context = context;
            _ingredientService = ingredientService;
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
            
            var nguyenLieu = await _ingredientService.GetIngredientByIdAsNguyenLieuAsync(ID);
            if (nguyenLieu == null)
            {
                TempData["UpdateNguyenLieuError"] = "Không tìm thấy nguyên liệu hoặc đã bị xóa.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            
            // Kiểm tra trùng mã (trừ chính nó)
            if (await _ingredientService.IngredientCodeExistsAsync(MaNguyenLieu, ID))
            {
                TempData["UpdateNguyenLieuError"] = "Mã nguyên liệu đã tồn tại.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            
            nguyenLieu.TenNguyenLieu = TenNguyenLieu.Trim();
            nguyenLieu.MaNguyenLieu = MaNguyenLieu.Trim();
            nguyenLieu.DonViTinh = DonViTinh.Trim();
            
            await _ingredientService.UpdateIngredientAsync(nguyenLieu);
            TempData["UpdateNguyenLieuSuccess"] = "Cập nhật nguyên liệu thành công!";
            return RedirectToPage("/NguyenLieu/NguyenLieuList");
        }
    }
}
