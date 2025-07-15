using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QRB.Pages.ProductGroup
{
    public class DeleteGroupModel : PageModel
    {
        private readonly QRBDbContext _context;
        public DeleteGroupModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Guid ID { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Đọc ID từ body JSON (trường hợp gọi AJAX với body { id: ... })
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            Guid id = ID;
            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var obj = System.Text.Json.JsonDocument.Parse(body);
                    if (obj.RootElement.TryGetProperty("id", out var idProp))
                    {
                        if (Guid.TryParse(idProp.GetString(), out var parsedId))
                        {
                            id = parsedId;
                        }
                    }
                }
                catch { }
            }
            if (id == Guid.Empty)
            {
                return new JsonResult(new { success = false, message = "ID không hợp lệ" });
            }
            var group = _context.NhomSanPhams.FirstOrDefault(x => x.ID == id);
            if (group == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy nhóm sản phẩm" });
            }
            group.IsDelete = true;
            group.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }
}
