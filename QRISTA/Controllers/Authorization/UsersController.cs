using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;
using QRB.Models.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace QRB.Controllers.Authorization
{
    [ApiController]
    [Route("api/authorization/users")]
    public class UsersController : ControllerBase
    {
        private readonly QRBDbContext _context;

        public UsersController(QRBDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(id));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = user.ID,
                username = user.TenNguoiDung,
                role = user.VaiTro,
                isActive = user.TrangThaiHoatDong,
                email = user.Email,
                fullName = user.TenHienThi
            });
        }

        [HttpPost("{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(id));
            if (user == null)
            {
                return NotFound();
            }

            user.TrangThaiHoatDong = !user.TrangThaiHoatDong;
            user.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Trạng thái người dùng đã được cập nhật", isActive = user.TrangThaiHoatDong });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(id));
            if (user == null)
            {
                return NotFound();
            }

            user.VaiTro = request.Role;
            user.TrangThaiHoatDong = request.IsActive;
            user.Email = request.Email;
            user.TenHienThi = request.FullName ?? user.TenHienThi;
            user.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thông tin người dùng thành công" });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            // Kiểm tra username đã tồn tại
            var existingUser = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenNguoiDung == request.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });
            }

            // Lấy chi nhánh đầu tiên (hoặc theo logic của bạn)
            var firstBranch = await _context.ChiNhanhs.FirstOrDefaultAsync();
            if (firstBranch == null)
            {
                return BadRequest(new { message = "Không tìm thấy chi nhánh để gán cho người dùng" });
            }

            var user = new NguoiDung
            {
                ID = Guid.NewGuid(),
                TenNguoiDung = request.Username,
                MatKhau = HashPassword(request.Password),
                VaiTro = request.Role,
                Email = request.Email,
                TenHienThi = request.FullName ?? request.Username,
                IDChiNhanh = firstBranch.ID,
                TrangThaiHoatDong = true,
                CreateTime = DateTime.Now,
                IsDelete = false
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo người dùng thành công", userId = user.ID });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(id));
            if (user == null)
            {
                return NotFound();
            }

            // Soft delete
            user.IsDelete = true;
            user.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa người dùng thành công" });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
