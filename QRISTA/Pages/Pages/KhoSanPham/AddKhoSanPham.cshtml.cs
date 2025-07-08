using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Threading.Tasks;

namespace QRB.Pages.KhoSanPham
{
    [IgnoreAntiforgeryToken]
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
        
        public async Task<IActionResult> OnPostAddAsync([FromBody] AddKhoSanPhamRequest data)
        {
            try
            {
                if (data?.IDNguyenLieu == null || data.SoLuongConLai <= 0 || data.IDChiNhanh == null)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ thông tin và số lượng phải > 0." });
                }

                // Kiểm tra nguyên liệu và chi nhánh có tồn tại không
                var nguyenLieu = await _context.NguyenLieus.FindAsync(data.IDNguyenLieu);
                var chiNhanh = await _context.ChiNhanhs.FindAsync(data.IDChiNhanh);
                
                if (nguyenLieu == null || nguyenLieu.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Nguyên liệu không tồn tại hoặc đã bị xóa." });
                }
                
                if (chiNhanh == null || chiNhanh.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Chi nhánh không tồn tại hoặc đã bị xóa." });
                }

                // Kiểm tra xem đã có kho sản phẩm này chưa
                var existingKho = _context.KhoSanPhams
                    .FirstOrDefault(k => k.IDNguyenLieu == data.IDNguyenLieu && k.IDChiNhanh == data.IDChiNhanh && !k.IsDelete);
                
                if (existingKho != null)
                {
                    return new JsonResult(new { success = false, message = "Nguyên liệu này đã tồn tại trong kho của chi nhánh này." });
                }

                var kho = new QRB.Models.KhoSanPham
                {
                    ID = Guid.NewGuid(),
                    IDNguyenLieu = data.IDNguyenLieu.Value,
                    SoLuongConLai = data.SoLuongConLai.ToString(),
                    IDChiNhanh = data.IDChiNhanh.Value,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                };
                
                _context.KhoSanPhams.Add(kho);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Thêm kho sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
    
    public class AddKhoSanPhamRequest
    {
        public Guid? IDNguyenLieu { get; set; }
        public int SoLuongConLai { get; set; }
        public Guid? IDChiNhanh { get; set; }
    }
}
