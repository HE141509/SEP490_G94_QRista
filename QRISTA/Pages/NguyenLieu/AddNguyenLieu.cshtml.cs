using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using QRB.Services;
using System.Threading.Tasks;

namespace QRB.Pages.NguyenLieu
{
    public class AddNguyenLieuModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly IIngredientService _ingredientService;
        
        public AddNguyenLieuModel(QRBDbContext context, IIngredientService ingredientService)
        {
            _context = context;
            _ingredientService = ingredientService;
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
            
            // Kiểm tra mã nguyên liệu đã tồn tại
            if (await _ingredientService.IngredientCodeExistsAsync(MaNguyenLieu))
            {
                TempData["AddNguyenLieuError"] = "Mã nguyên liệu đã tồn tại.";
                return RedirectToPage("/NguyenLieu/NguyenLieuList");
            }
            
            var nguyenLieu = new QRB.Models.NguyenLieu
            {
                TenNguyenLieu = TenNguyenLieu.Trim(),
                MaNguyenLieu = MaNguyenLieu.Trim(),
                DonViTinh = DonViTinh.Trim()
            };

            await _ingredientService.AddIngredientAsync(nguyenLieu);
            TempData["AddNguyenLieuSuccess"] = "Thêm nguyên liệu thành công!";
            return RedirectToPage("/NguyenLieu/NguyenLieuList");
        }
    }
}
