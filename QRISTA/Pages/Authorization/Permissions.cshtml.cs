using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models.Authorization;

namespace QRB.Pages.Authorization
{
    public class PermissionsModel : PageModel
    {
        private readonly QRBDbContext _context;

        public PermissionsModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<AppRole> Roles { get; set; } = new List<AppRole>();
        public List<AppPermission> Permissions { get; set; } = new List<AppPermission>();
        public List<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
        public Dictionary<string, List<AppPermission>> PermissionGroups { get; set; } = new Dictionary<string, List<AppPermission>>();
        public string CurrentUserRole { get; private set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToPage("/Login");
            }

            // Lấy vai trò người dùng từ session
            CurrentUserRole = HttpContext.Session.GetString("VaiTro") ?? "Người dùng";

            try
            {
                // Lấy dữ liệu từ database
                Roles = await _context.Roles.Where(r => r.IsActive == true).OrderBy(r => r.Name).ToListAsync();
                Permissions = await _context.Permissions.OrderBy(p => p.Module ?? "ZZZ").ThenBy(p => p.Name).ToListAsync();
                RolePermissions = await _context.RolePermissions.ToListAsync();

                // Nếu không có permissions, tạo dữ liệu mẫu
                if (!Permissions.Any())
                {
                    await CreateSamplePermissionsIfNeeded();
                    Permissions = await _context.Permissions.OrderBy(p => p.Module ?? "ZZZ").ThenBy(p => p.Name).ToListAsync();
                }

                // Nếu không có role permissions, tạo dữ liệu mẫu
                if (!RolePermissions.Any())
                {
                    await CreateSampleRolePermissionsIfNeeded();
                    RolePermissions = await _context.RolePermissions.ToListAsync();
                }

                // Nếu không có roles, sử dụng dữ liệu mẫu
                if (!Roles.Any())
                {
                    await CreateSampleRolesIfNeeded();
                    Roles = await _context.Roles.Where(r => r.IsActive == true).OrderBy(r => r.Name).ToListAsync();
                }

                // Nhóm permissions theo module
                PermissionGroups = Permissions.GroupBy(p => p.Module ?? "Other")
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch (Exception)
            {
                // Nếu có lỗi, tạo dữ liệu mẫu
                await CreateSampleRolesIfNeeded();
                await CreateSamplePermissionsIfNeeded();
                await CreateSampleRolePermissionsIfNeeded();
                
                Roles = await _context.Roles.Where(r => r.IsActive == true).OrderBy(r => r.Name).ToListAsync();
                Permissions = await _context.Permissions.OrderBy(p => p.Module ?? "ZZZ").ThenBy(p => p.Name).ToListAsync();
                RolePermissions = await _context.RolePermissions.ToListAsync();
                
                PermissionGroups = Permissions.GroupBy(p => p.Module ?? "Other")
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            
            return Page();
        }

        private async Task CreateSampleRolesIfNeeded()
        {
            if (!await _context.Roles.AnyAsync())
            {
                var sampleRoles = new List<AppRole>
                {
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Admin", Description = "Quản trị viên hệ thống", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Manager", Description = "Quản lý chi nhánh", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Staff", Description = "Nhân viên bán hàng", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Cashier", Description = "Thu ngân", IsActive = true }
                };

                _context.Roles.AddRange(sampleRoles);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateSamplePermissionsIfNeeded()
        {
            if (!await _context.Permissions.AnyAsync())
            {
                var samplePermissions = new List<AppPermission>
                {
                    // Module Quản lý người dùng
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "View Users", Description = "Xem danh sách người dùng", Module = "User Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Create Users", Description = "Tạo người dùng mới", Module = "User Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Edit Users", Description = "Chỉnh sửa thông tin người dùng", Module = "User Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Delete Users", Description = "Xóa người dùng", Module = "User Management" },
                    
                    // Module Quản lý sản phẩm
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "View Products", Description = "Xem danh sách sản phẩm", Module = "Product Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Create Products", Description = "Tạo sản phẩm mới", Module = "Product Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Edit Products", Description = "Chỉnh sửa thông tin sản phẩm", Module = "Product Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Delete Products", Description = "Xóa sản phẩm", Module = "Product Management" },
                    
                    // Module Quản lý đơn hàng
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "View Orders", Description = "Xem danh sách đơn hàng", Module = "Order Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Create Orders", Description = "Tạo đơn hàng mới", Module = "Order Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Edit Orders", Description = "Chỉnh sửa đơn hàng", Module = "Order Management" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Cancel Orders", Description = "Hủy đơn hàng", Module = "Order Management" },
                    
                    // Module Báo cáo
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "View Reports", Description = "Xem báo cáo", Module = "Reports" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Export Reports", Description = "Xuất báo cáo", Module = "Reports" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Financial Reports", Description = "Báo cáo tài chính", Module = "Reports" },
                    
                    // Module Hệ thống
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "System Settings", Description = "Cài đặt hệ thống", Module = "System" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "Backup Database", Description = "Sao lưu dữ liệu", Module = "System" },
                    new AppPermission { Id = Guid.NewGuid().ToString(), Name = "View Logs", Description = "Xem nhật ký hệ thống", Module = "System" }
                };

                _context.Permissions.AddRange(samplePermissions);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateSampleRolePermissionsIfNeeded()
        {
            if (!await _context.RolePermissions.AnyAsync())
            {
                var roles = await _context.Roles.ToListAsync();
                var permissions = await _context.Permissions.ToListAsync();

                if (roles.Any() && permissions.Any())
                {
                    var adminRole = roles.FirstOrDefault(r => r.Name == "Admin");
                    if (adminRole != null)
                    {
                        // Admin có tất cả quyền
                        foreach (var permission in permissions)
                        {
                            _context.RolePermissions.Add(new AppRolePermission 
                            { 
                                Id = Guid.NewGuid().ToString(),
                                RoleId = adminRole.Id, 
                                PermissionId = permission.Id 
                            });
                        }
                    }

                    var managerRole = roles.FirstOrDefault(r => r.Name == "Manager");
                    if (managerRole != null)
                    {
                        // Manager có quyền quản lý sản phẩm, đơn hàng và xem báo cáo
                        var managerPermissionNames = new[] { "View Products", "Create Products", "Edit Products", 
                                                           "View Orders", "Create Orders", "Edit Orders", 
                                                           "View Reports", "Export Reports" };
                        foreach (var permissionName in managerPermissionNames)
                        {
                            var permission = permissions.FirstOrDefault(p => p.Name == permissionName);
                            if (permission != null)
                            {
                                _context.RolePermissions.Add(new AppRolePermission 
                                { 
                                    Id = Guid.NewGuid().ToString(),
                                    RoleId = managerRole.Id, 
                                    PermissionId = permission.Id 
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
