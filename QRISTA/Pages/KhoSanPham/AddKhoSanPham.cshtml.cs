using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using Microsoft.EntityFrameworkCore;
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
                if (data?.IDNguyenLieu == null || data.SoLuongConLai <= 0)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ thông tin và số lượng phải > 0." });
                }

                // Lấy thông tin chi nhánh của user đang đăng nhập
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
                {
                    return new JsonResult(new { success = false, message = "Vui lòng đăng nhập lại." });
                }

                var currentUser = await _context.NguoiDungs
                    .Where(u => u.ID == userGuid && !u.IsDelete)
                    .FirstOrDefaultAsync();
                
                if (currentUser == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy thông tin người dùng." });
                }

                var userBranchId = currentUser.IDChiNhanh;

                // Kiểm tra nguyên liệu có tồn tại không
                var nguyenLieu = await _context.Ingredients.FindAsync(data.IDNguyenLieu);
                
                if (nguyenLieu == null || nguyenLieu.IsDeleted)
                {
                    return new JsonResult(new { success = false, message = "Nguyên liệu không tồn tại hoặc đã bị xóa." });
                }

                // Kiểm tra xem đã có kho sản phẩm này chưa (sử dụng chi nhánh của user hiện tại)
                var existingKho = await _context.KhoSanPhams
                    .FirstOrDefaultAsync(k => k.IDNguyenLieu == data.IDNguyenLieu && k.IDChiNhanh == userBranchId && !k.IsDelete);
                
                if (existingKho != null)
                {
                    // Nếu đã tồn tại, cộng thêm số lượng mới vào số lượng cũ
                    if (int.TryParse(existingKho.SoLuongConLai, out var currentQuantity))
                    {
                        var newTotalQuantity = currentQuantity + data.SoLuongConLai;
                        existingKho.SoLuongConLai = newTotalQuantity.ToString();
                        existingKho.UpdateTime = DateTime.Now;
                        
                        _context.KhoSanPhams.Update(existingKho);
                        await _context.SaveChangesAsync();
                        return new JsonResult(new { success = true, message = $"Đã cập nhật số lượng! Số lượng hiện tại: {newTotalQuantity:N0}" });
                    }
                    else
                    {
                        return new JsonResult(new { success = false, message = "Lỗi dữ liệu số lượng trong cơ sở dữ liệu." });
                    }
                }

                // Nếu chưa tồn tại, thêm mới (sử dụng chi nhánh của user hiện tại)
                var kho = new QRB.Models.KhoSanPham
                {
                    ID = Guid.NewGuid(),
                    IDNguyenLieu = data.IDNguyenLieu.Value,
                    SoLuongConLai = data.SoLuongConLai.ToString(),
                    IDChiNhanh = userBranchId,
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
    }
}
