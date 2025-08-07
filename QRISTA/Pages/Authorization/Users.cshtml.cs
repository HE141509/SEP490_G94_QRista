using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Pages.Authorization
{
    public class UsersModel : PageModel
    {
        private readonly QRBDbContext _context;

        public UsersModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<NguoiDung> Users { get; set; } = new List<NguoiDung>();
        public string CurrentUserRole { get; private set; } = string.Empty;
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string RoleFilter { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            // Kiểm tra đăng nhập
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToPage("/Login");
            }

            // Lấy vai trò người dùng từ session
            CurrentUserRole = HttpContext.Session.GetString("VaiTro") ?? "Người dùng";

            CurrentPage = pageNumber;
            PageSize = pageSize;

            var query = _context.NguoiDungs.Where(u => !u.IsDelete).AsQueryable();

            // Áp dụng bộ lọc
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(u => u.TenNguoiDung.Contains(SearchTerm) ||
                                        (u.TenHienThi != null && u.TenHienThi.Contains(SearchTerm)) ||
                                        (u.Email != null && u.Email.Contains(SearchTerm)));
            }

            if (!string.IsNullOrEmpty(RoleFilter))
            {
                query = query.Where(u => u.VaiTro == RoleFilter);
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                bool isActive = StatusFilter == "active";
                query = query.Where(u => u.TrangThaiHoatDong == isActive);
            }

            TotalRecords = await query.CountAsync();

            // LẤY TẤT CẢ DỮ LIỆU để JavaScript phân trang client-side
            Users = await query
                .OrderBy(u => u.TenNguoiDung)
                .ToListAsync();

            // Nếu không có dữ liệu và không có bộ lọc, tạo dữ liệu mẫu
            if (!Users.Any() && string.IsNullOrEmpty(SearchTerm) && string.IsNullOrEmpty(RoleFilter) && string.IsNullOrEmpty(StatusFilter))
            {
                await CreateSampleUsersIfNeeded();
                Users = await query
                    .OrderBy(u => u.TenNguoiDung)
                    .ToListAsync();
                TotalRecords = await query.CountAsync();
            }

            return Page();
        }

        private async Task CreateSampleUsersIfNeeded()
        {
            if (!await _context.NguoiDungs.AnyAsync(u => !u.IsDelete))
            {
                var sampleUsers = new List<NguoiDung>();
                
                // Tạo 25 user mẫu để test phân trang
                var roles = new[] { "Admin", "Manager", "Staff", "Cashier" };
                var names = new[] { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Phan", "Vũ", "Võ", "Đặng", "Bùi" };
                var firstName = new[] { "Văn", "Thị", "Đức", "Minh", "Hồng", "Tuấn", "Linh", "Hương", "Sơn", "Mai" };
                
                for (int i = 0; i < 25; i++)
                {
                    var role = roles[i % roles.Length];
                    var lastName = names[i % names.Length];
                    var first = firstName[i % firstName.Length];
                    var username = $"user{i + 1:D2}";
                    var displayName = $"{lastName} {first} {i + 1}";
                    var email = $"{username}@qrb.com";
                    
                    sampleUsers.Add(new NguoiDung
                    {
                        ID = Guid.NewGuid(),
                        TenNguoiDung = username,
                        TenHienThi = displayName,
                        Email = email,
                        VaiTro = role,
                        TrangThaiHoatDong = i % 5 != 0, // 80% active, 20% inactive
                        CreateTime = DateTime.Now.AddDays(-i),
                        IsDelete = false
                    });
                }

                _context.NguoiDungs.AddRange(sampleUsers);
                await _context.SaveChangesAsync();
            }
        }
    }
}
