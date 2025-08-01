using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models.Authorization;

namespace QRB.Controllers.Authorization
{
    [ApiController]
    [Route("api/authorization/roles")]
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
                return BadRequest(new { success = false, message = "Vai trò này đã tồn tại" });
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

            return Ok(new { success = true, message = "Tạo vai trò thành công", roleId = role.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy vai trò" });
            }

            role.Name = request.Name;
            role.Description = request.Description;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật vai trò thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy vai trò" });
            }

            // Không cho phép xóa vai trò Admin
            if (role.Name == "Admin")
            {
                return BadRequest(new { success = false, message = "Không thể xóa vai trò Admin" });
            }

            // Xóa các permissions liên quan
            var rolePermissions = await _context.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
            _context.RolePermissions.RemoveRange(rolePermissions);

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Xóa vai trò thành công" });
        }

        [HttpPost("{id}/toggle-status")]
        public async Task<IActionResult> ToggleRoleStatus(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy vai trò" });
            }

            // Không cho phép vô hiệu hóa vai trò Admin
            if (role.Name == "Admin" && role.IsActive == true)
            {
                return BadRequest(new { success = false, message = "Không thể vô hiệu hóa vai trò Admin" });
            }

            // Chuyển đổi trạng thái
            role.IsActive = !role.IsActive;
            await _context.SaveChangesAsync();

            string statusMessage = role.IsActive == true ? "kích hoạt" : "vô hiệu hóa";
            return Ok(new { success = true, message = $"Đã {statusMessage} vai trò thành công" });
        }
    }
}
