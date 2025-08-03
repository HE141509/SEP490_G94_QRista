using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using System;
using System.Threading.Tasks;

namespace QRB.Pages.DeXuatMuaSam
{
    [IgnoreAntiforgeryToken]
    public class DeleteDeXuatMuaSamModel : PageModel
    {
        private readonly QRBDbContext _context;
        public DeleteDeXuatMuaSamModel(QRBDbContext context)
        {
            _context = context;
        }

        public class DeleteInput
        {
            public Guid id { get; set; }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            try
            {
                string body = await new StreamReader(Request.Body).ReadToEndAsync();
                var input = System.Text.Json.JsonSerializer.Deserialize<DeleteInput>(body);
                if (input == null)
                    return new JsonResult(new { success = false, message = $"Không nhận được body hoặc body không hợp lệ: {body}" });
                if (input.id == Guid.Empty)
                    return new JsonResult(new { success = false, message = $"ID rỗng hoặc không hợp lệ! Body: {body}" });

                var deXuat = await _context.DeXuatMuaSams.FindAsync(input.id);
                if (deXuat == null)
                    return new JsonResult(new { success = false, message = $"Không tìm thấy đề xuất mua sắm với id: {input.id}" });

                // Kiểm tra trạng thái trước khi xóa
                if (deXuat.Status == "accepted" || deXuat.Status == "received")
                {
                    return new JsonResult(new { success = false, message = "Không thể xóa đề xuất đã được duyệt hoặc đã nhận." });
                }

                deXuat.IsDelete = true;
                deXuat.UpdateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
