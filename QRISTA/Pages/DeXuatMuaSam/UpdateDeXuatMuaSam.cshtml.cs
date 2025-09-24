using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;
using System.Text.Json;
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
        
        public async Task<IActionResult> OnPostAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy dữ liệu từ form
                string? idStr = Request.Form["id"];
                string? tieuDe = Request.Form["tieuDe"];
                string? noiDungDeXuat = Request.Form["noiDungDeXuat"];
                string? idChiNhanhNhanStr = Request.Form["idChiNhanhNhan"];
                string? idNguoiNhanStr = Request.Form["idNguoiNhan"];

                if (string.IsNullOrEmpty(idStr) || !Guid.TryParse(idStr, out Guid id))
                {
                    return new JsonResult(new { success = false, message = "ID không hợp lệ" });
                }

                if (string.IsNullOrEmpty(tieuDe) || string.IsNullOrEmpty(noiDungDeXuat) ||
                    string.IsNullOrEmpty(idChiNhanhNhanStr) || string.IsNullOrEmpty(idNguoiNhanStr))
                {
                    return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ thông tin" });
                }

                if (!Guid.TryParse(idChiNhanhNhanStr, out Guid idChiNhanhNhan) ||
                    !Guid.TryParse(idNguoiNhanStr, out Guid idNguoiNhan))
                {
                    return new JsonResult(new { success = false, message = "Chi nhánh hoặc người nhận không hợp lệ" });
                }

                var deXuat = await _context.DeXuatMuaSams
                    .Where(dx => dx.ID == id && !dx.IsDelete)
                    .FirstOrDefaultAsync();

                if (deXuat == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy phiếu đề xuất" });
                }

                // Kiểm tra chỉ cho phép cập nhật khi ở trạng thái "Chờ duyệt"
                if (deXuat.Status != "pending")
                {
                    return new JsonResult(new { success = false, message = "Chỉ có thể cập nhật đề xuất ở trạng thái 'Chờ duyệt'" });
                }

                // Cập nhật thông tin
                deXuat.TieuDe = tieuDe;
                deXuat.NoiDungDeXuat = noiDungDeXuat;
                deXuat.IDChiNhanhNhan = idChiNhanhNhan;
                deXuat.IDNguoiNhan = idNguoiNhan;
                deXuat.UpdateTime = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new JsonResult(new { success = true, message = "Cập nhật phiếu đề xuất thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateNguyenLieuAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string? idDeXuatStr = Request.Form["idDeXuat"];
                string? nguyenLieuListJson = Request.Form["NguyenLieuList"];

                if (string.IsNullOrEmpty(idDeXuatStr) || !Guid.TryParse(idDeXuatStr, out Guid idDeXuat))
                {
                    return new JsonResult(new { success = false, message = "ID đề xuất không hợp lệ" });
                }

                if (string.IsNullOrEmpty(nguyenLieuListJson))
                {
                    return new JsonResult(new { success = false, message = "Danh sách nguyên liệu không được để trống" });
                }

                var nguyenLieuList = JsonSerializer.Deserialize<List<UpdateNguyenLieuItem>>(nguyenLieuListJson);
                if (nguyenLieuList == null || !nguyenLieuList.Any())
                {
                    return new JsonResult(new { success = false, message = "Danh sách nguyên liệu không hợp lệ" });
                }

                // Kiểm tra đề xuất có tồn tại và đang ở trạng thái pending
                var deXuat = await _context.DeXuatMuaSams
                    .Where(dx => dx.ID == idDeXuat && !dx.IsDelete)
                    .FirstOrDefaultAsync();

                if (deXuat == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy phiếu đề xuất" });
                }

                if (deXuat.Status != "pending")
                {
                    return new JsonResult(new { success = false, message = "Chỉ có thể cập nhật nguyên liệu khi đề xuất ở trạng thái 'Chờ duyệt'" });
                }

                // Xóa các chi tiết cũ
                var existingDetails = await _context.ChiTietDonDeXuats
                    .Where(ct => ct.IDDeXuatMuaSam == idDeXuat)
                    .ToListAsync();

                _context.ChiTietDonDeXuats.RemoveRange(existingDetails);

                // Thêm chi tiết mới
                foreach (var nguyenLieu in nguyenLieuList)
                {
                    if (string.IsNullOrEmpty(nguyenLieu.Id) || !Guid.TryParse(nguyenLieu.Id, out Guid idNguyenLieu) || nguyenLieu.SoLuong <= 0)
                        continue;

                    var chiTiet = new ChiTietDonDeXuat
                    {
                        ID = Guid.NewGuid(),
                        IDDeXuatMuaSam = idDeXuat,
                        IDNguyenLieu = idNguyenLieu,
                        SoLuong = nguyenLieu.SoLuong,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        IsDelete = false
                    };

                    await _context.ChiTietDonDeXuats.AddAsync(chiTiet);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new JsonResult(new { success = true, message = "Cập nhật danh sách nguyên liệu thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
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
                    return new JsonResult(new { success = false, message = "Không tìm thấy phiếu đề xuất." });
                }

                // Kiểm tra chỉ cho phép cập nhật khi ở trạng thái "Chờ duyệt"
                if (deXuat.Status != "pending")
                {
                    return new JsonResult(new { success = false, message = "Chỉ có thể cập nhật đề xuất ở trạng thái 'Chờ duyệt'." });
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
                return new JsonResult(new { success = true, message = "Cập nhật phiếu đề xuất thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }

    public class UpdateNguyenLieuItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public string? DonViTinh { get; set; }
    }
    
    public class UpdateDeXuatMuaSamRequest
    {
        public Guid ID { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string NoiDungDeXuat { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid IDChiNhanhNhan { get; set; }
        public Guid IDNguoiNhan { get; set; }
    }
}
