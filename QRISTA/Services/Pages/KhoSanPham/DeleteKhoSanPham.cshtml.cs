using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using System;
using System.Threading.Tasks;

namespace QRB.Pages.KhoSanPham
{
    [IgnoreAntiforgeryToken]
    public class DeleteKhoSanPhamModel : PageModel
    {
        private readonly QRBDbContext _context;
        public DeleteKhoSanPhamModel(QRBDbContext context)
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

                // Trả về id nhận được để debug
                // return new JsonResult(new { success = false, message = $"ID nhận được: {input.id}" });

                var kho = await _context.KhoSanPhams.FindAsync(input.id);
                if (kho == null)
                    return new JsonResult(new { success = false, message = $"Không tìm thấy KhoSanPham với id: {input.id}" });

                // Kiểm tra số lượng trước khi xóa
                if (int.TryParse(kho.SoLuongConLai, out var soLuong) && soLuong > 0)
                {
                    return new JsonResult(new { success = false, message = $"Không thể xóa vì còn tồn kho {soLuong:N0} đơn vị. Vui lòng xuất hết hàng trước khi xóa." });
                }

                kho.IsDelete = true;
                kho.UpdateTime = DateTime.Now;
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
