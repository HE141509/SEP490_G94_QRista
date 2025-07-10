using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Threading.Tasks;
using System.Text.Json;

namespace QRB.Pages.KhoSanPham
{
    [IgnoreAntiforgeryToken]
    public class UpdateKhoSanPhamModel : PageModel
    {
        private readonly QRBDbContext _context;
        public UpdateKhoSanPhamModel(QRBDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Guid ID { get; set; }
        [BindProperty]
        public Guid IDNguyenLieu { get; set; }
        [BindProperty]
        public string SoLuongConLai { get; set; } = string.Empty;
        [BindProperty]
        public Guid IDChiNhanh { get; set; }
        
        public IActionResult OnGet() => RedirectToPage("/KhoSanPham/KhoSanPhamList");
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (ID == Guid.Empty || IDNguyenLieu == Guid.Empty || string.IsNullOrWhiteSpace(SoLuongConLai) || !int.TryParse(SoLuongConLai, out var sl) || sl <= 0 || IDChiNhanh == Guid.Empty)
            {
                TempData["UpdateKhoSanPhamError"] = "Vui lòng nhập đầy đủ thông tin và số lượng phải > 0.";
                return RedirectToPage("/KhoSanPham/KhoSanPhamList");
            }
            var kho = await _context.KhoSanPhams.FindAsync(ID);
            if (kho == null || kho.IsDelete)
            {
                TempData["UpdateKhoSanPhamError"] = "Không tìm thấy kho sản phẩm hoặc đã bị xóa.";
                return RedirectToPage("/KhoSanPham/KhoSanPhamList");
            }
            kho.IDNguyenLieu = IDNguyenLieu;
            kho.SoLuongConLai = SoLuongConLai;
            kho.IDChiNhanh = IDChiNhanh;
            kho.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["UpdateKhoSanPhamSuccess"] = "Cập nhật kho sản phẩm thành công!";
            return RedirectToPage("/KhoSanPham/KhoSanPhamList");
        }
        
        public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateKhoSanPhamRequest data)
        {
            try
            {
                if (data?.Id == null || data.IDNguyenLieu == null || data.SoLuongConLai <= 0 || data.IDChiNhanh == null)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ thông tin và số lượng phải > 0." });
                }

                var kho = await _context.KhoSanPhams.FindAsync(data.Id);
                if (kho == null || kho.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy kho sản phẩm hoặc đã bị xóa." });
                }

                kho.IDNguyenLieu = data.IDNguyenLieu.Value;
                kho.SoLuongConLai = data.SoLuongConLai.ToString();
                kho.IDChiNhanh = data.IDChiNhanh.Value;
                kho.UpdateTime = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Cập nhật kho sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
    
    public class UpdateKhoSanPhamRequest
    {
        public Guid? Id { get; set; }
        public Guid? IDNguyenLieu { get; set; }
        public int SoLuongConLai { get; set; }
        public Guid? IDChiNhanh { get; set; }
    }
}
