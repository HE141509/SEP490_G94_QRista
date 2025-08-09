using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace QRB.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string userId, string permissionName);
        Task<bool> HasPermissionAsync(string userId, string permissionName, string module);
        Task<List<string>> GetUserPermissionsAsync(string userId);
        Task<List<AppPermission>> GetUserPermissionDetailsAsync(string userId);
        bool HasPermissionFromSession(ISession session, string permissionName);
        List<string> GetUserPermissionsFromSession(ISession session);
    }

    public class PermissionService : IPermissionService
    {
        private readonly QRBDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(QRBDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> HasPermissionAsync(string userId, string permissionName)
        {
            try
            {
                // Lấy user và role
                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.ID.ToString() == userId);
                if (user == null) return false;

                // Tìm role của user
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.VaiTro && r.IsActive == true);
                if (role == null) return false;

                // Kiểm tra permission
                var hasPermission = await _context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == role.Id && 
                                   rp.Permission.Name == permissionName);

                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", permissionName, userId);
                return false;
            }
        }

        public async Task<bool> HasPermissionAsync(string userId, string permissionName, string module)
        {
            try
            {
                // Lấy user và role
                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.ID.ToString() == userId);
                if (user == null) return false;

                // Tìm role của user
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.VaiTro && r.IsActive == true);
                if (role == null) return false;

                // Kiểm tra permission với module
                var hasPermission = await _context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == role.Id && 
                                   rp.Permission.Name == permissionName && 
                                   rp.Permission.Module == module);

                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} in module {Module} for user {UserId}", permissionName, module, userId);
                return false;
            }
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            try
            {
                // Lấy user và role
                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.ID.ToString() == userId);
                if (user == null) return new List<string>();

                // Tìm role của user
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.VaiTro && r.IsActive == true);
                if (role == null) return new List<string>();

                // Lấy tất cả permissions của role
                var permissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => rp.Permission.Name)
                    .ToListAsync();

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
                return new List<string>();
            }
        }

        public async Task<List<AppPermission>> GetUserPermissionDetailsAsync(string userId)
        {
            try
            {
                // Lấy user và role
                var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.ID.ToString() == userId);
                if (user == null) return new List<AppPermission>();

                // Tìm role của user
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.VaiTro && r.IsActive == true);
                if (role == null) return new List<AppPermission>();

                // Lấy tất cả permission details của role
                var permissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => rp.Permission)
                    .ToListAsync();

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission details for user {UserId}", userId);
                return new List<AppPermission>();
            }
        }

        public bool HasPermissionFromSession(ISession session, string permissionName)
        {
            try
            {
                var permissionsJson = session.GetString("UserPermissions");
                if (string.IsNullOrEmpty(permissionsJson))
                    return false;

                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
                return permissions?.Contains(permissionName) == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} from session", permissionName);
                return false;
            }
        }

        public List<string> GetUserPermissionsFromSession(ISession session)
        {
            try
            {
                var permissionsJson = session.GetString("UserPermissions");
                if (string.IsNullOrEmpty(permissionsJson))
                    return new List<string>();

                return JsonSerializer.Deserialize<List<string>>(permissionsJson) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions from session");
                return new List<string>();
            }
        }
    }
}
