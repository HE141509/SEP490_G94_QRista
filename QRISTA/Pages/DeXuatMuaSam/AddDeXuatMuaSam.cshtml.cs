using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Threading.Tasks;

namespace QRB.Pages.DeXuatMuaSam
{
    [IgnoreAntiforgeryToken]
    public class AddDeXuatMuaSamModel : PageModel
    {
        private readonly QRBDbContext _context;
        public AddDeXuatMuaSamModel(QRBDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet() => RedirectToPage("/DeXuatMuaSam/DeXuatMuaSamList");
        
        public async Task<IActionResult> OnPostAddAsync([FromBody] AddDeXuatMuaSamRequest data)
        {
            try
            {
                if (data == null || string.IsNullOrWhiteSpace(data.MaDeXuat) || string.IsNullOrWhiteSpace(data.TieuDe) || 
                    string.IsNullOrWhiteSpace(data.NoiDungDeXuat) || data.IDNguoiGui == null || data.IDChiNhanhGui == null ||
                    data.IDNguoiNhan == null || data.IDChiNhanhNhan == null)
                {
                    return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
                }

                // Kiểm tra mã đề xuất đã tồn tại chưa
                var existingDeXuat = _context.DeXuatMuaSams
                    .FirstOrDefault(dx => dx.MaDeXuat == data.MaDeXuat && !dx.IsDelete);
                
                if (existingDeXuat != null)
                {
                    return new JsonResult(new { success = false, message = "Mã đề xuất đã tồn tại." });
                }

                // Kiểm tra người dùng và chi nhánh có tồn tại không
                var nguoiGui = await _context.NguoiDungs.FindAsync(data.IDNguoiGui);
                var chiNhanhGui = await _context.ChiNhanhs.FindAsync(data.IDChiNhanhGui);
                var nguoiNhan = await _context.NguoiDungs.FindAsync(data.IDNguoiNhan);
                var chiNhanhNhan = await _context.ChiNhanhs.FindAsync(data.IDChiNhanhNhan);
                
                if (nguoiGui == null || nguoiGui.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Người gửi không tồn tại hoặc đã bị xóa." });
                }
                
                if (chiNhanhGui == null || chiNhanhGui.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Chi nhánh gửi không tồn tại hoặc đã bị xóa." });
                }

                if (nguoiNhan == null || nguoiNhan.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Người nhận không tồn tại hoặc đã bị xóa." });
                }
                
                if (chiNhanhNhan == null || chiNhanhNhan.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Chi nhánh nhận không tồn tại hoặc đã bị xóa." });
                }

                var deXuat = new QRB.Models.DeXuatMuaSam
                {
                    ID = Guid.NewGuid(),
                    IDNguoiGui = data.IDNguoiGui.Value,
                    IDChiNhanhGui = data.IDChiNhanhGui.Value,
                    IDNguoiNhan = data.IDNguoiNhan.Value,
                    IDChiNhanhNhan = data.IDChiNhanhNhan.Value,
                    MaDeXuat = data.MaDeXuat,
                    TieuDe = data.TieuDe,
                    NoiDungDeXuat = data.NoiDungDeXuat,
                    Status = "pending", // Mặc định là chờ duyệt
                    CreateTime = DateTime.Now,
                    IsDelete = false
                };
                
                _context.DeXuatMuaSams.Add(deXuat);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Thêm đề xuất mua sắm thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
    
    public class AddDeXuatMuaSamRequest
    {
        public string MaDeXuat { get; set; } = string.Empty;
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDungDeXuat { get; set; } = string.Empty;
        public Guid? IDNguoiGui { get; set; }
        public Guid? IDChiNhanhGui { get; set; }
        public Guid? IDNguoiNhan { get; set; }
        public Guid? IDChiNhanhNhan { get; set; }
    }
}
