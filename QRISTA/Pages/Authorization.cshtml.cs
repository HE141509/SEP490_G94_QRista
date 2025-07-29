using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using Microsoft.EntityFrameworkCore;

namespace QRB.Pages
{
    public class AuthorizationModel : PageModel
    {
        private readonly QRBDbContext _context;

        public AuthorizationModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<NguoiDung> Users { get; set; } = new List<NguoiDung>();
        public List<AppRole> Roles { get; set; } = new List<AppRole>();
        public List<AppPermission> Permissions { get; set; } = new List<AppPermission>();
        public Dictionary<string, List<string>> RolePermissions { get; set; } = new Dictionary<string, List<string>>();

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostCreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["Error"] = "Tên vai trò không được để trống";
                return RedirectToPage();
            }

            // Kiểm tra vai trò đã tồn tại
            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (existingRole != null)
            {
                TempData["Error"] = "Vai trò này đã tồn tại";
                return RedirectToPage();
            }

            var newRole = new AppRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = roleName,
                Description = $"Vai trò {roleName}",
                CreatedAt = DateTime.Now
            };

            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm vai trò thành công";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateUserAsync(string userId, string username, string role, bool isActive)
        {
            var user = await _context.NguoiDungs.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng";
                return RedirectToPage();
            }

            user.VaiTro = role;
            user.TrangThaiHoatDong = isActive;
            user.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thông tin người dùng thành công";
            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Load Users - xử lý NULL values
                Users = await _context.NguoiDungs
                    .Include(u => u.ChiNhanh)
                    .Where(u => !u.IsDelete)
                    .Select(u => new NguoiDung
                    {
                        ID = u.ID,
                        TenNguoiDung = u.TenNguoiDung,
                        TenHienThi = u.TenHienThi,
                        VaiTro = u.VaiTro ?? "Staff",
                        TrangThaiHoatDong = u.TrangThaiHoatDong,
                        Email = u.Email,
                        IDChiNhanh = u.IDChiNhanh,
                        CreateTime = u.CreateTime,
                        UpdateTime = u.UpdateTime,
                        IsDelete = u.IsDelete,
                        ChiNhanh = u.ChiNhanh
                    })
                    .OrderBy(u => u.TenNguoiDung)
                    .ToListAsync();

                // Load Roles
                Roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();

                // Load Permissions
                Permissions = await LoadPermissions();

                // Load Role-Permission mappings
                await LoadRolePermissions();
            }
            catch (Exception ex)
            {
                // Log error và set default values
                Console.WriteLine($"Error loading authorization data: {ex.Message}");
                Users = new List<NguoiDung>();
                Roles = new List<AppRole>();
                Permissions = new List<AppPermission>();
                RolePermissions = new Dictionary<string, List<string>>();
            }
        }

        private async Task<List<AppPermission>> LoadPermissions()
        {
            // Kiểm tra xem bảng Permission có tồn tại không
            var permissions = new List<AppPermission>();
            
            try
            {
                permissions = await _context.Permissions.ToListAsync();
                
                // Nếu không có permissions nào, tạo mặc định
                if (!permissions.Any())
                {
                    await CreateDefaultPermissions();
                    permissions = await _context.Permissions.ToListAsync();
                }
            }
            catch
            {
                // Nếu bảng chưa tồn tại, tạo permissions mặc định
                await CreateDefaultPermissions();
                permissions = await _context.Permissions.ToListAsync();
            }

            return permissions;
        }

        private async Task CreateDefaultPermissions()
        {
            var defaultPermissions = new List<AppPermission>
            {
                new AppPermission { Id = "1", Name = "Quản lý người dùng", Description = "Thêm, sửa, xóa người dùng", Category = "User Management" },
                new AppPermission { Id = "2", Name = "Quản lý sản phẩm", Description = "Quản lý menu và sản phẩm", Category = "Product Management" },
                new AppPermission { Id = "3", Name = "Quản lý đơn hàng", Description = "Xem và xử lý đơn hàng", Category = "Order Management" },
                new AppPermission { Id = "4", Name = "Quản lý kho", Description = "Quản lý tồn kho và nhập xuất", Category = "Inventory Management" },
                new AppPermission { Id = "5", Name = "Quản lý chi nhánh", Description = "Thêm, sửa, xóa chi nhánh", Category = "Branch Management" },
                new AppPermission { Id = "6", Name = "Quản lý khuyến mãi", Description = "Tạo và quản lý ưu đãi", Category = "Promotion Management" },
                new AppPermission { Id = "7", Name = "Báo cáo thống kê", Description = "Xem báo cáo và thống kê", Category = "Reporting" },
                new AppPermission { Id = "8", Name = "Cấu hình hệ thống", Description = "Cấu hình tham số hệ thống", Category = "System Configuration" },
                new AppPermission { Id = "9", Name = "Quản lý thanh toán", Description = "Xử lý thanh toán và hoàn tiền", Category = "Payment Management" },
                new AppPermission { Id = "10", Name = "Quản lý khách hàng", Description = "Quản lý thông tin khách hàng", Category = "Customer Management" }
            };

            foreach (var permission in defaultPermissions)
            {
                var existing = await _context.Permissions.FindAsync(permission.Id);
                if (existing == null)
                {
                    _context.Permissions.Add(permission);
                }
            }
            
            await _context.SaveChangesAsync();
        }

        private async Task LoadRolePermissions()
        {
            RolePermissions = new Dictionary<string, List<string>>();

            try
            {
                var rolePermissionMappings = await _context.RolePermissions.ToListAsync();
                
                foreach (var role in Roles)
                {
                    var permissionIds = rolePermissionMappings
                        .Where(rp => rp.RoleId == role.Id)
                        .Select(rp => rp.PermissionId)
                        .ToList();
                    
                    RolePermissions[role.Id] = permissionIds;
                }
            }
            catch
            {
                // Nếu bảng chưa tồn tại, tạo permissions mặc định cho Admin
                foreach (var role in Roles)
                {
                    if (role.Name == "Admin")
                    {
                        RolePermissions[role.Id] = Permissions.Select(p => p.Id).ToList();
                    }
                    else
                    {
                        RolePermissions[role.Id] = new List<string> { "2", "3", "10" }; // Permissions cơ bản cho staff
                    }
                }
            }
        }

        public bool HasPermission(string roleId, string permissionId)
        {
            return RolePermissions.ContainsKey(roleId) && 
                   RolePermissions[roleId].Contains(permissionId);
        }
    }
}
