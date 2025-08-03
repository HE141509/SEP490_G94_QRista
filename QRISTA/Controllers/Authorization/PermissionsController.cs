using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models.Authorization;

namespace QRB.Controllers.Authorization
{
    [ApiController]
    [Route("api/authorization/permissions")]
    public class PermissionsController : ControllerBase
    {
        private readonly QRBDbContext _context;

        public PermissionsController(QRBDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            var permissions = await _context.Permissions.OrderBy(p => p.Module ?? "ZZZ").ThenBy(p => p.Name).ToListAsync();
            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermission(string id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy quyền" });
            }

            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            var existingPermission = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == request.Name);
            if (existingPermission != null)
            {
                return BadRequest(new { success = false, message = "Quyền này đã tồn tại" });
            }

            var permission = new AppPermission
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                Module = request.Module,
                CreatedAt = DateTime.Now
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Tạo quyền thành công", permissionId = permission.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(string id, [FromBody] UpdatePermissionRequest request)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy quyền" });
            }

            permission.Name = request.Name;
            permission.Description = request.Description;
            permission.Module = request.Module;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật quyền thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(string id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy quyền" });
            }

            // Xóa các role permissions liên quan
            var rolePermissions = await _context.RolePermissions.Where(rp => rp.PermissionId == id).ToListAsync();
            _context.RolePermissions.RemoveRange(rolePermissions);

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Xóa quyền thành công" });
        }

        [HttpPost("update-matrix")]
        public async Task<IActionResult> UpdatePermissionMatrix([FromBody] List<PermissionChangeRequest> changes)
        {
            try
            {
                foreach (var change in changes)
                {
                    var existingPermission = await _context.RolePermissions
                        .FirstOrDefaultAsync(rp => rp.RoleId == change.RoleId && rp.PermissionId == change.PermissionId);

                    if (change.IsGranted && existingPermission == null)
                    {
                        // Thêm quyền mới
                        var newPermission = new AppRolePermission
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleId = change.RoleId,
                            PermissionId = change.PermissionId,
                            GrantedAt = DateTime.Now
                        };
                        _context.RolePermissions.Add(newPermission);
                    }
                    else if (!change.IsGranted && existingPermission != null)
                    {
                        // Xóa quyền
                        _context.RolePermissions.Remove(existingPermission);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cập nhật ma trận phân quyền thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet("matrix")]
        public async Task<IActionResult> GetPermissionMatrix()
        {
            var roles = await _context.Roles.Where(r => r.IsActive == true).OrderBy(r => r.Name).ToListAsync();
            var permissions = await _context.Permissions.OrderBy(p => p.Module ?? "ZZZ").ThenBy(p => p.Name).ToListAsync();
            var rolePermissions = await _context.RolePermissions.ToListAsync();

            var matrix = new
            {
                roles = roles,
                permissions = permissions,
                assignments = rolePermissions.ToDictionary(
                    rp => $"{rp.RoleId}_{rp.PermissionId}",
                    rp => true
                )
            };

            return Ok(matrix);
        }
    }
}