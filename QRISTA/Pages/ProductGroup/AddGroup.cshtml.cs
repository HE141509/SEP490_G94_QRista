using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QRB.Pages.ProductGroup
{
    public class AddGroupModel : PageModel
    {
        private readonly QRBDbContext _context;
        public AddGroupModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty]
    public Category? Group { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Đọc body JSON nếu có (AJAX gửi trực tiếp JSON thay vì form)
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            Category? input = Group;
            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var doc = System.Text.Json.JsonDocument.Parse(body);
                    var root = doc.RootElement;
                    input = new Category {
                        CategoryCode = root.TryGetProperty("CategoryCode", out var maNhomProp) ? maNhomProp.GetString() : null,
                        CategoryName = root.TryGetProperty("CategoryName", out var tenNhomProp) ? tenNhomProp.GetString() : null,
                        IsDelete = root.TryGetProperty("IsDelete", out var isDeleteProp) && isDeleteProp.ValueKind == System.Text.Json.JsonValueKind.True ? true : false
                    };
                }
                catch { }
            }
            if (input == null || string.IsNullOrWhiteSpace(input.CategoryCode) || string.IsNullOrWhiteSpace(input.CategoryName))
            {
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            input.ID = Guid.NewGuid();
            input.CreateTime = DateTime.Now;
            input.UpdateTime = null;
            _context.Categories.Add(input);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }
}
