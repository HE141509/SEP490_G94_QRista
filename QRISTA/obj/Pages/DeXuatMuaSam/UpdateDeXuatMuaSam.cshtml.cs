using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using System.Threading.Tasks;

namespace QRB.Pages.DeXuatMuaSam
{
    [IgnoreAntiforgeryToken]
    public class UpdateDeXuatMuaSamModel : PageModel
    {
        private readonly QRBDbContext _context;
        public UpdateDeXuatMuaSamModel(QRBDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var deXuat = _context.DeXuatMuaSams
                .FirstOrDefault(dx => dx.MaDeXuat == id && !dx.IsDelete);
            
            if (deXuat == null)
            {
                return NotFound();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateDeXuatMuaSamRequest data)
        {
            try
            {
                if (data == null || data.ID == Guid.Empty || string.IsNullOrWhiteSpace(data.TieuDe) || 
                    string.IsNullOrWhiteSpace(data.NoiDungDeXuat) || string.IsNullOrWhiteSpace(data.Status))
                {
                    return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
                }

                var deXuat = await _context.DeXuatMuaSams.FindAsync(data.ID);
                if (deXuat == null || deXuat.IsDelete)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đề xuất mua sắm." });
                }

                // Cập nhật thông tin
                deXuat.TieuDe = data.TieuDe;
                deXuat.NoiDungDeXuat = data.NoiDungDeXuat;
                deXuat.Status = data.Status;
                deXuat.UpdateTime = DateTime.Now;

                // Cập nhật thời gian duyệt/nhận nếu cần
                if (data.Status == "accepted" && deXuat.AcceptTime == null)
                {
                    deXuat.AcceptTime = DateTime.Now;
                }
                else if (data.Status == "received" && deXuat.ReceiveTime == null)
                {
                    deXuat.ReceiveTime = DateTime.Now;
                }

                _context.DeXuatMuaSams.Update(deXuat);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Cập nhật đề xuất mua sắm thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
    
    public class UpdateDeXuatMuaSamRequest
    {
        public Guid ID { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDungDeXuat { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
