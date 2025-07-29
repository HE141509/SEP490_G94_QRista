using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;
using System.Security.Cryptography;
using System.Text;

namespace QRB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly QRBDbContext _context;

        public RolesController(QRBDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Name);
            if (existingRole != null)
            {
                return BadRequest(new { message = "Vai trò này đã tồn tại" });
            }

            var role = new AppRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.Now
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tạo vai trò thành công", roleId = role.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.Name = request.Name;
            role.Description = request.Description;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật vai trò thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Không cho phép xóa vai trò Admin
            if (role.Name == "Admin")
            {
                return BadRequest(new { message = "Không thể xóa vai trò Admin" });
            }

            // Xóa các permissions liên quan
            var rolePermissions = await _context.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
            _context.RolePermissions.RemoveRange(rolePermissions);

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa vai trò thành công" });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly QRBDbContext _context;

        public PermissionsController(QRBDbContext context)
        {
            _context = context;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> TogglePermission([FromBody] TogglePermissionRequest request)
        {
            if (request.HasPermission)
            {
                // Thêm quyền
                var existingPermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId);

                if (existingPermission == null)
                {
                    var rolePermission = new AppRolePermission
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoleId = request.RoleId,
                        PermissionId = request.PermissionId,
                        CreatedAt = DateTime.Now
                    };

                    _context.RolePermissions.Add(rolePermission);
                }
            }
            else
            {
                // Xóa quyền
                var existingPermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId);

                if (existingPermission != null)
                {
                    _context.RolePermissions.Remove(existingPermission);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật quyền thành công" });
        }
    }

    // Request models
    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Staff";
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class TogglePermissionRequest
    {
        public string RoleId { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public bool HasPermission { get; set; }
    }
}
