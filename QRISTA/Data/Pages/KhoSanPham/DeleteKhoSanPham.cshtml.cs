using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using Microsoft.EntityFrameworkCore;
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

                var kho = await _context.KhoSanPhams.FindAsync(input.id);
                if (kho == null)
                    return new JsonResult(new { success = false, message = $"Không tìm thấy KhoSanPham với id: {input.id}" });

                // Kiểm tra quyền: chỉ được xóa kho sản phẩm thuộc chi nhánh của mình
                if (kho.IDChiNhanh != userBranchId)
                {
                    return new JsonResult(new { success = false, message = "Bạn không có quyền xóa kho sản phẩm này." });
                }

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
