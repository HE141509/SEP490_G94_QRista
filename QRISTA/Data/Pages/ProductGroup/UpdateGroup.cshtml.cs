using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QRB.Pages.ProductGroup
{
    public class UpdateGroupModel : PageModel
    {
        private readonly QRBDbContext _context;
        public UpdateGroupModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NhomSanPham? Group { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Đọc body JSON nếu có (AJAX gửi trực tiếp JSON thay vì form)
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            NhomSanPham? input = Group;
            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var doc = System.Text.Json.JsonDocument.Parse(body);
                    var root = doc.RootElement;
                    input = new NhomSanPham {
                        ID = root.TryGetProperty("ID", out var idProp) && Guid.TryParse(idProp.GetString(), out var gid) ? gid : Guid.Empty,
                        MaNhom = root.TryGetProperty("MaNhom", out var maNhomProp) ? maNhomProp.GetString() : null,
                        TenNhom = root.TryGetProperty("TenNhom", out var tenNhomProp) ? tenNhomProp.GetString() : null,
                        IsDelete = root.TryGetProperty("IsDelete", out var isDeleteProp) && isDeleteProp.ValueKind == System.Text.Json.JsonValueKind.True ? true : false
                    };
                }
                catch { }
            }
            if (input == null || input.ID == Guid.Empty)
            {
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            var group = _context.NhomSanPhams.FirstOrDefault(x => x.ID == input.ID);
            if (group == null)
            {
                return new JsonResult(new { success = false, message = "Không tìm thấy nhóm sản phẩm" });
            }
            group.MaNhom = input.MaNhom;
            group.TenNhom = input.TenNhom;
            group.IsDelete = input.IsDelete;
            group.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }
}
