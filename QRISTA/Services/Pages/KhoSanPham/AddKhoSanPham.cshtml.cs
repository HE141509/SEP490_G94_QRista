using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Threading.Tasks;

namespace QRB.Pages.KhoSanPham
{
    public class AddKhoSanPhamModel : PageModel
    {
        private readonly QRBDbContext _context;
        public AddKhoSanPhamModel(QRBDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Guid IDNguyenLieu { get; set; }
        [BindProperty]
        public string SoLuongConLai { get; set; } = string.Empty;
        [BindProperty]
        public Guid IDChiNhanh { get; set; }
        public IActionResult OnGet() => RedirectToPage("/KhoSanPham/KhoSanPhamList");
        public async Task<IActionResult> OnPostAsync()
        {
            if (IDNguyenLieu == Guid.Empty || string.IsNullOrWhiteSpace(SoLuongConLai) || !int.TryParse(SoLuongConLai, out var sl) || sl <= 0 || IDChiNhanh == Guid.Empty)
            {
                TempData["AddKhoSanPhamError"] = "Vui lòng nhập đầy đủ thông tin và số lượng phải > 0.";
                return RedirectToPage("/KhoSanPham/KhoSanPhamList");
            }
           var kho = new QRB.Models.KhoSanPham
           {
               ID = Guid.NewGuid(),
               IDNguyenLieu = IDNguyenLieu,
               SoLuongConLai = SoLuongConLai,
               IDChiNhanh = IDChiNhanh,
               IsDelete = false,
               CreateTime = DateTime.Now
           };
            _context.KhoSanPhams.Add(kho);
            await _context.SaveChangesAsync();
            TempData["AddKhoSanPhamSuccess"] = "Thêm kho sản phẩm thành công!";
            return RedirectToPage("/KhoSanPham/KhoSanPhamList");
        }
    }
}
