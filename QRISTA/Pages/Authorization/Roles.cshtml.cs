using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models.Authorization;

namespace QRB.Pages.Authorization
{
    public class RolesModel : PageModel
    {
        private readonly QRBDbContext _context;

        public RolesModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<RoleWithUserCount> Roles { get; set; } = new List<RoleWithUserCount>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                // Lấy tất cả roles để JavaScript phân trang client-side
                var roles = await _context.Roles.ToListAsync();
                
                Roles = roles.Select(r => new RoleWithUserCount
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive ?? true, // Xử lý NULL thành true
                    UserCount = _context.NguoiDungs.Count(u => u.VaiTro == r.Name && !u.IsDelete)
                }).ToList();
                
                // Nếu không có dữ liệu, tạo dữ liệu mẫu
                if (!Roles.Any())
                {
                    await CreateSampleRolesIfNeeded();
                    
                    roles = await _context.Roles.ToListAsync();
                    Roles = roles.Select(r => new RoleWithUserCount
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        IsActive = r.IsActive ?? true,
                        UserCount = _context.NguoiDungs.Count(u => u.VaiTro == r.Name && !u.IsDelete)
                    }).ToList();
                }
            }
            catch (Exception)
            {
                // Nếu lỗi, tạo dữ liệu mẫu
                await CreateSampleRolesIfNeeded();
                
                var roles = await _context.Roles.ToListAsync();
                Roles = roles.Select(r => new RoleWithUserCount
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive ?? true, // Xử lý NULL thành true
                    UserCount = _context.NguoiDungs.Count(u => u.VaiTro == r.Name && !u.IsDelete)
                }).ToList();
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
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Manager", Description = "Quản lý cửa hàng", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Staff", Description = "Nhân viên bán hàng", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Cashier", Description = "Thu ngân", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Supervisor", Description = "Giám sát ca làm việc", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Accountant", Description = "Kế toán", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Marketing", Description = "Nhân viên marketing", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "HR", Description = "Nhân viên nhân sự", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Customer Support", Description = "Hỗ trợ khách hàng", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Inventory Manager", Description = "Quản lý kho", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Chef", Description = "Đầu bếp", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Waiter", Description = "Nhân viên phục vụ", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Security", Description = "Bảo vệ", IsActive = false },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Cleaner", Description = "Nhân viên vệ sinh", IsActive = true },
                    new AppRole { Id = Guid.NewGuid().ToString(), Name = "Delivery", Description = "Nhân viên giao hàng", IsActive = true }
                };

                _context.Roles.AddRange(sampleRoles);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class RoleWithUserCount
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int UserCount { get; set; }
    }
}
