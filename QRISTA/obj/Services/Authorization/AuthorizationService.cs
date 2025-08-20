using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;
using QRB.Models.Authorization;

namespace QRB.Services.Authorization
{
    public interface IAuthorizationService
    {
        Task<bool> HasPermissionAsync(string userId, string permissionId);
        Task<bool> HasPermissionAsync(string userId, string[] permissionIds);
        Task<List<string>> GetUserPermissionsAsync(string userId);
        Task<UserViewModel?> GetUserViewModelAsync(Guid userId);
        Task<List<RoleViewModel>> GetRoleViewModelsAsync();
        Task<List<PermissionViewModel>> GetPermissionViewModelsAsync(string? roleId = null);
    }

    public class AuthorizationService : IAuthorizationService
    {
        private readonly QRBDbContext _context;

        public AuthorizationService(QRBDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(string userId, string permissionId)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(userId));
            if (user == null || !user.TrangThaiHoatDong)
                return false;

            // Tìm role của user
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.VaiTro);
            if (role == null)
                return false;

            // Kiểm tra permission
            var hasPermission = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permissionId);

            return hasPermission;
        }

        public async Task<bool> HasPermissionAsync(string userId, string[] permissionIds)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(userId));
            if (user == null || !user.TrangThaiHoatDong)
                return false;

            // Tìm role của user
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.VaiTro);
            if (role == null)
                return false;

            // Kiểm tra tất cả permissions
            var userPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            return permissionIds.All(pid => userPermissions.Contains(pid));
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(userId));
            if (user == null || !user.TrangThaiHoatDong)
                return new List<string>();

            // Tìm role của user
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == user.VaiTro);
            if (role == null)
                return new List<string>();

            // Lấy tất cả permissions
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == role.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            return permissions;
        }

        public async Task<UserViewModel?> GetUserViewModelAsync(Guid userId)
        {
            var user = await _context.NguoiDungs
                .Include(u => u.ChiNhanh)
                .FirstOrDefaultAsync(u => u.ID == userId && !u.IsDelete);

            if (user == null)
                return null;

            return new UserViewModel
            {
                Id = user.ID,
                Username = user.TenNguoiDung,
                DisplayName = user.TenHienThi,
                Role = user.VaiTro ?? "Staff",
                IsActive = user.TrangThaiHoatDong,
                Email = user.Email,
                BranchName = user.ChiNhanh?.TenChiNhanh ?? "",
                CreatedAt = user.CreateTime
            };
        }

        public async Task<List<RoleViewModel>> GetRoleViewModelsAsync()
        {
            var roles = await _context.Roles
                .Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    UserCount = _context.NguoiDungs.Count(u => u.VaiTro == r.Name && !u.IsDelete),
                    PermissionCount = _context.RolePermissions.Count(rp => rp.RoleId == r.Id),
                    CreatedAt = r.CreatedAt
                })
                .OrderBy(r => r.Name)
                .ToListAsync();

            return roles;
        }

        public async Task<List<PermissionViewModel>> GetPermissionViewModelsAsync(string? roleId = null)
        {
            var permissions = await _context.Permissions
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();

            var rolePermissions = new List<string>();
            if (!string.IsNullOrEmpty(roleId))
            {
                rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();
            }

            return permissions.Select(p => new PermissionViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Module = p.Module,
                IsAssigned = rolePermissions.Contains(p.Id)
            }).ToList();
        }
    }
}
