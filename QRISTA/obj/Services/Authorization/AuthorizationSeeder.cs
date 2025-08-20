using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;
using QRB.Models.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace QRB.Services.Authorization
{
    public class AuthorizationSeeder
    {
        private readonly QRBDbContext _context;

        public AuthorizationSeeder(QRBDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Đảm bảo database được tạo
                await _context.Database.EnsureCreatedAsync();

                await SeedRoles();
                await SeedPermissions();
                await SeedUsers();
                await SeedRolePermissions();
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                Console.WriteLine($"Error seeding authorization data: {ex.Message}");
            }
        }

        private async Task SeedRoles()
        {
            var roles = new[]
            {
                new AppRole { Id = "admin-role", Name = "Admin", Description = "Quản trị viên hệ thống" },
                new AppRole { Id = "manager-role", Name = "Manager", Description = "Quản lý cửa hàng" },
                new AppRole { Id = "staff-role", Name = "Staff", Description = "Nhân viên bán hàng" },
                new AppRole { Id = "cashier-role", Name = "Cashier", Description = "Thu ngân" }
            };

            foreach (var role in roles)
            {
                var existingRole = await _context.Roles.FindAsync(role.Id);
                if (existingRole == null)
                {
                    _context.Roles.Add(role);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedPermissions()
        {
            var permissions = new[]
            {
                new AppPermission { Id = "user-management", Name = "Quản lý người dùng", Description = "Thêm, sửa, xóa người dùng", Module = "User Management" },
                new AppPermission { Id = "product-management", Name = "Quản lý sản phẩm", Description = "Quản lý menu và sản phẩm", Module = "Product Management" },
                new AppPermission { Id = "order-management", Name = "Quản lý đơn hàng", Description = "Xem và xử lý đơn hàng", Module = "Order Management" },
                new AppPermission { Id = "inventory-management", Name = "Quản lý kho", Description = "Quản lý tồn kho và nhập xuất", Module = "Inventory Management" },
                new AppPermission { Id = "branch-management", Name = "Quản lý chi nhánh", Description = "Thêm, sửa, xóa chi nhánh", Module = "Branch Management" },
                new AppPermission { Id = "promotion-management", Name = "Quản lý khuyến mãi", Description = "Tạo và quản lý ưu đãi", Module = "Promotion Management" },
                new AppPermission { Id = "reporting", Name = "Báo cáo thống kê", Description = "Xem báo cáo và thống kê", Module = "Reporting" },
                new AppPermission { Id = "system-config", Name = "Cấu hình hệ thống", Description = "Cấu hình tham số hệ thống", Module = "System Configuration" },
                new AppPermission { Id = "payment-management", Name = "Quản lý thanh toán", Description = "Xử lý thanh toán và hoàn tiền", Module = "Payment Management" },
                new AppPermission { Id = "customer-management", Name = "Quản lý khách hàng", Description = "Quản lý thông tin khách hàng", Module = "Customer Management" },
                new AppPermission { Id = "order-create", Name = "Tạo đơn hàng", Description = "Tạo đơn hàng mới", Module = "Order Management" },
                new AppPermission { Id = "order-view", Name = "Xem đơn hàng", Description = "Xem thông tin đơn hàng", Module = "Order Management" },
                new AppPermission { Id = "menu-view", Name = "Xem menu", Description = "Xem danh sách sản phẩm", Module = "Product Management" },
                new AppPermission { Id = "cashier-operations", Name = "Thao tác thu ngân", Description = "Thu tiền và in hóa đơn", Module = "Payment Management" }
            };

            foreach (var permission in permissions)
            {
                var existingPermission = await _context.Permissions.FindAsync(permission.Id);
                if (existingPermission == null)
                {
                    _context.Permissions.Add(permission);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedUsers()
        {
            // Lấy chi nhánh đầu tiên để gán cho người dùng
            var firstBranch = await _context.ChiNhanhs.FirstOrDefaultAsync();
            if (firstBranch == null)
            {
                Console.WriteLine("No branch found for user assignment");
                return;
            }

            var users = new[]
            {
                new NguoiDung 
                { 
                    ID = Guid.NewGuid(), 
                    TenNguoiDung = "admin", 
                    MatKhau = HashPassword("123456"),
                    VaiTro = "Admin",
                    TenHienThi = "Quản trị viên",
                    Email = "admin@qrb.com",
                    TrangThaiHoatDong = true,
                    IDChiNhanh = firstBranch.ID,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                },
                new NguoiDung 
                { 
                    ID = Guid.NewGuid(), 
                    TenNguoiDung = "staff", 
                    MatKhau = HashPassword("123456"),
                    VaiTro = "Staff",
                    TenHienThi = "Nhân viên",
                    Email = "staff@qrb.com",
                    TrangThaiHoatDong = true,
                    IDChiNhanh = firstBranch.ID,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                },
                new NguoiDung 
                { 
                    ID = Guid.NewGuid(), 
                    TenNguoiDung = "manager", 
                    MatKhau = HashPassword("123456"),
                    VaiTro = "Manager",
                    TenHienThi = "Quản lý",
                    Email = "manager@qrb.com",
                    TrangThaiHoatDong = true,
                    IDChiNhanh = firstBranch.ID,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                }
            };

            foreach (var user in users)
            {
                var existingUser = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenNguoiDung == user.TenNguoiDung);
                if (existingUser == null)
                {
                    _context.NguoiDungs.Add(user);
                }
                else
                {
                    // Cập nhật thông tin nếu user đã tồn tại
                    existingUser.VaiTro = user.VaiTro;
                    existingUser.Email = user.Email;
                    existingUser.TenHienThi = user.TenHienThi;
                    existingUser.TrangThaiHoatDong = user.TrangThaiHoatDong;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedRolePermissions()
        {
            // Admin có tất cả quyền
            var adminPermissions = await _context.Permissions.Select(p => p.Id).ToListAsync();
            await AssignPermissionsToRole("admin-role", adminPermissions);

            // Manager có một số quyền
            var managerPermissions = new[]
            {
                "product-management", "order-management", "inventory-management",
                "promotion-management", "reporting", "customer-management",
                "order-create", "order-view", "menu-view", "payment-management"
            };
            await AssignPermissionsToRole("manager-role", managerPermissions);

            // Staff có quyền cơ bản
            var staffPermissions = new[]
            {
                "order-create", "order-view", "menu-view", "customer-management"
            };
            await AssignPermissionsToRole("staff-role", staffPermissions);

            // Cashier có quyền thu ngân
            var cashierPermissions = new[]
            {
                "order-view", "menu-view", "cashier-operations", "payment-management"
            };
            await AssignPermissionsToRole("cashier-role", cashierPermissions);

            await _context.SaveChangesAsync();
        }

        private async Task AssignPermissionsToRole(string roleId, string[] permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                var existing = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

                if (existing == null)
                {
                    _context.RolePermissions.Add(new AppRolePermission
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoleId = roleId,
                        PermissionId = permissionId,
                        GrantedAt = DateTime.Now
                    });
                }
            }
        }

        private async Task AssignPermissionsToRole(string roleId, List<string> permissionIds)
        {
            await AssignPermissionsToRole(roleId, permissionIds.ToArray());
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
