using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;
using QRB.Services;
using System.Text.Json;

namespace QRB.Pages
{
    public class LoginModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly IConfiguration _config;
        private readonly IPermissionService _permissionService;

        public LoginModel(QRBDbContext context, IConfiguration config, IPermissionService permissionService)
        {
            _context = context;
            _config = config;
            _permissionService = permissionService;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // Kiểm tra nếu đã đăng nhập thì chuyển về Dashboard
            if (HttpContext.Session.GetString("UserId") != null)
            {
                Response.Redirect("/Dashboard");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Kiểm tra tài khoản trong database
                var user = await _context.NguoiDungs
                    .Include(u => u.ChiNhanh)
                    .FirstOrDefaultAsync(u => u.TenNguoiDung == Username && !u.IsDelete);

                if (user != null && VerifyPassword(Password, user.MatKhau))
                {
                    // Kiểm tra tài khoản có bị khóa không
                    if (!user.TrangThaiHoatDong)
                    {
                        ErrorMessage = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên!";
                        return Page();
                    }

                    // Đăng nhập thành công
                    HttpContext.Session.SetString("UserId", user.ID.ToString());
                    HttpContext.Session.SetString("Username", user.TenNguoiDung);
                    HttpContext.Session.SetString("DisplayName", user.TenHienThi);
                    HttpContext.Session.SetString("ChiNhanhId", user.IDChiNhanh.ToString());
                    HttpContext.Session.SetString("ChiNhanhName", user.ChiNhanh.DepartmentName);
                    HttpContext.Session.SetString("VaiTro", user.VaiTro); // Thêm vai trò vào session
                    
                    // Lấy tất cả các permissions của user từ RolePermissions và lưu vào session
                    var userPermissions = await _permissionService.GetUserPermissionsAsync(user.ID.ToString());
                    var permissionsJson = JsonSerializer.Serialize(userPermissions);
                    HttpContext.Session.SetString("UserPermissions", permissionsJson);

                    if (RememberMe)
                    {
                        // Set cookie for remember me (30 days)
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(30),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        };
                        Response.Cookies.Append("RememberMe", user.ID.ToString(), cookieOptions);
                    }

                    return RedirectToPage("/Dashboard");
                }
                else
                {
                    // Kiểm tra tài khoản demo
                    if (Username == "admin" && Password == "123456")
                    {
                        var adminUserId = Guid.NewGuid().ToString();
                        HttpContext.Session.SetString("UserId", adminUserId);
                        HttpContext.Session.SetString("Username", "admin");
                        HttpContext.Session.SetString("DisplayName", "Quản trị viên");
                        HttpContext.Session.SetString("ChiNhanhId", Guid.NewGuid().ToString());
                        HttpContext.Session.SetString("ChiNhanhName", "QRB Coffee - Chi nhánh chính");
                        HttpContext.Session.SetString("VaiTro", "Quản trị viên");
                        
                        // Tạo permissions demo cho admin (full access)
                        var adminPermissions = new List<string> {
                            "View Dashboard", "View Users", "Create Users", "Update Users", "Delete Users",
                            "View Roles", "Create Roles", "Update Roles", "Delete Roles",
                            "View Permissions", "Assign Permissions",
                            "View Customers", "Create Customers", "Update Customers", "Delete Customers",
                            "View Branches", "Create Branches", "Update Branches", "Delete Branches"
                        };
                        HttpContext.Session.SetString("UserPermissions", JsonSerializer.Serialize(adminPermissions));
                        
                        return RedirectToPage("/Dashboard");
                    }
                    else if (Username == "staff" && Password == "123456")
                    {
                        var staffUserId = Guid.NewGuid().ToString();
                        HttpContext.Session.SetString("UserId", staffUserId);
                        HttpContext.Session.SetString("Username", "staff");
                        HttpContext.Session.SetString("DisplayName", "Nhân viên");
                        HttpContext.Session.SetString("ChiNhanhId", Guid.NewGuid().ToString());
                        HttpContext.Session.SetString("ChiNhanhName", "QRB Coffee - Chi nhánh chính");
                        HttpContext.Session.SetString("VaiTro", "Nhân viên");
                        
                        // Tạo permissions demo cho staff (limited access)
                        var staffPermissions = new List<string> {
                            "View Dashboard", "View Customers", "Create Customers", "Update Customers",
                            "View Orders", "Create Orders", "Update Orders"
                        };
                        HttpContext.Session.SetString("UserPermissions", JsonSerializer.Serialize(staffPermissions));
                        
                        return RedirectToPage("/Dashboard");
                    }
                    else
                    {
                        ErrorMessage = "Tài khoản hoặc mật khẩu không đúng!";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại!";
                // Log error for debugging
                Console.WriteLine($"Login error: {ex.Message}");
            }

            return Page();
        }

        private string HashPassword(string password, string key)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            var key = _config["PasswordKey"] ?? string.Empty;
            var hash = HashPassword(inputPassword, key);
            return hash == hashedPassword;
        }
    }
}
