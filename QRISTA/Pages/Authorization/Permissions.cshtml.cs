using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models.Authorization;

namespace QRB.Pages.Authorization
{
    public class PermissionsModel : PageModel
    {
        public List<AppRole> Roles { get; set; } = new List<AppRole>();
        public List<AppPermission> Permissions { get; set; } = new List<AppPermission>();
        public List<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
        public Dictionary<string, List<AppPermission>> PermissionGroups { get; set; } = new Dictionary<string, List<AppPermission>>();

        public async Task OnGetAsync()
        {
            // Tạo dữ liệu mẫu cho roles
            Roles = new List<AppRole>
            {
                new AppRole { Id = "1", Name = "Admin", Description = "Quản trị viên hệ thống", IsActive = true },
                new AppRole { Id = "2", Name = "Manager", Description = "Quản lý chi nhánh", IsActive = true },
                new AppRole { Id = "3", Name = "Staff", Description = "Nhân viên bán hàng", IsActive = true },
                new AppRole { Id = "4", Name = "Cashier", Description = "Thu ngân", IsActive = true },
                new AppRole { Id = "5", Name = "Viewer", Description = "Chỉ xem báo cáo", IsActive = false }
            };

            // Tạo dữ liệu mẫu cho permissions
            Permissions = new List<AppPermission>
            {
                // Module Quản lý người dùng
                new AppPermission { Id = "1", Name = "View Users", Description = "Xem danh sách người dùng", Module = "User Management" },
                new AppPermission { Id = "2", Name = "Create Users", Description = "Tạo người dùng mới", Module = "User Management" },
                new AppPermission { Id = "3", Name = "Edit Users", Description = "Chỉnh sửa thông tin người dùng", Module = "User Management" },
                new AppPermission { Id = "4", Name = "Delete Users", Description = "Xóa người dùng", Module = "User Management" },
                
                // Module Quản lý sản phẩm
                new AppPermission { Id = "5", Name = "View Products", Description = "Xem danh sách sản phẩm", Module = "Product Management" },
                new AppPermission { Id = "6", Name = "Create Products", Description = "Tạo sản phẩm mới", Module = "Product Management" },
                new AppPermission { Id = "7", Name = "Edit Products", Description = "Chỉnh sửa thông tin sản phẩm", Module = "Product Management" },
                new AppPermission { Id = "8", Name = "Delete Products", Description = "Xóa sản phẩm", Module = "Product Management" },
                
                // Module Quản lý đơn hàng
                new AppPermission { Id = "9", Name = "View Orders", Description = "Xem danh sách đơn hàng", Module = "Order Management" },
                new AppPermission { Id = "10", Name = "Create Orders", Description = "Tạo đơn hàng mới", Module = "Order Management" },
                new AppPermission { Id = "11", Name = "Edit Orders", Description = "Chỉnh sửa đơn hàng", Module = "Order Management" },
                new AppPermission { Id = "12", Name = "Cancel Orders", Description = "Hủy đơn hàng", Module = "Order Management" },
                
                // Module Báo cáo
                new AppPermission { Id = "13", Name = "View Reports", Description = "Xem báo cáo", Module = "Reports" },
                new AppPermission { Id = "14", Name = "Export Reports", Description = "Xuất báo cáo", Module = "Reports" },
                new AppPermission { Id = "15", Name = "Financial Reports", Description = "Báo cáo tài chính", Module = "Reports" },
                
                // Module Hệ thống
                new AppPermission { Id = "16", Name = "System Settings", Description = "Cài đặt hệ thống", Module = "System" },
                new AppPermission { Id = "17", Name = "Backup Database", Description = "Sao lưu dữ liệu", Module = "System" },
                new AppPermission { Id = "18", Name = "View Logs", Description = "Xem nhật ký hệ thống", Module = "System" }
            };

            // Tạo dữ liệu mẫu cho role permissions (Admin có tất cả quyền)
            RolePermissions = new List<AppRolePermission>();
            
            // Admin có tất cả quyền
            foreach (var permission in Permissions)
            {
                RolePermissions.Add(new AppRolePermission 
                { 
                    Id = Guid.NewGuid().ToString(),
                    RoleId = "1", 
                    PermissionId = permission.Id 
                });
            }
            
            // Manager có quyền quản lý sản phẩm, đơn hàng và xem báo cáo
            var managerPermissions = new[] { "5", "6", "7", "9", "10", "11", "13", "14" };
            foreach (var permissionId in managerPermissions)
            {
                RolePermissions.Add(new AppRolePermission 
                { 
                    Id = Guid.NewGuid().ToString(),
                    RoleId = "2", 
                    PermissionId = permissionId 
                });
            }
            
            // Staff có quyền xem và tạo sản phẩm, đơn hàng
            var staffPermissions = new[] { "5", "6", "9", "10" };
            foreach (var permissionId in staffPermissions)
            {
                RolePermissions.Add(new AppRolePermission 
                { 
                    Id = Guid.NewGuid().ToString(),
                    RoleId = "3", 
                    PermissionId = permissionId 
                });
            }
            
            // Cashier chỉ có quyền xem và tạo đơn hàng
            var cashierPermissions = new[] { "9", "10" };
            foreach (var permissionId in cashierPermissions)
            {
                RolePermissions.Add(new AppRolePermission 
                { 
                    Id = Guid.NewGuid().ToString(),
                    RoleId = "4", 
                    PermissionId = permissionId 
                });
            }
            
            // Viewer chỉ xem báo cáo
            var viewerPermissions = new[] { "13" };
            foreach (var permissionId in viewerPermissions)
            {
                RolePermissions.Add(new AppRolePermission 
                { 
                    Id = Guid.NewGuid().ToString(),
                    RoleId = "5", 
                    PermissionId = permissionId 
                });
            }

            // Nhóm permissions theo module
            PermissionGroups = Permissions.GroupBy(p => p.Module)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            await Task.CompletedTask;
        }
    }
}
