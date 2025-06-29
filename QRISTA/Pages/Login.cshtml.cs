using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Pages
{
    public class LoginModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly IConfiguration _config;

        public LoginModel(QRBDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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
                    // Đăng nhập thành công
                    HttpContext.Session.SetString("UserId", user.ID.ToString());
                    HttpContext.Session.SetString("Username", user.TenNguoiDung);
                    HttpContext.Session.SetString("DisplayName", user.TenHienThi);
                    HttpContext.Session.SetString("ChiNhanhId", user.IDChiNhanh.ToString());
                    HttpContext.Session.SetString("ChiNhanhName", user.ChiNhanh.TenChiNhanh);

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
                        HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString());
                        HttpContext.Session.SetString("Username", "admin");
                        HttpContext.Session.SetString("DisplayName", "Quản trị viên");
                        HttpContext.Session.SetString("ChiNhanhId", Guid.NewGuid().ToString());
                        HttpContext.Session.SetString("ChiNhanhName", "QRB Coffee - Chi nhánh chính");
                        return RedirectToPage("/Dashboard");
                    }
                    else if (Username == "staff" && Password == "123456")
                    {
                        HttpContext.Session.SetString("UserId", Guid.NewGuid().ToString());
                        HttpContext.Session.SetString("Username", "staff");
                        HttpContext.Session.SetString("DisplayName", "Nhân viên");
                        HttpContext.Session.SetString("ChiNhanhId", Guid.NewGuid().ToString());
                        HttpContext.Session.SetString("ChiNhanhName", "QRB Coffee - Chi nhánh chính");
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
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + key);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            var key = _config["PasswordKey"];
            var hash = HashPassword(inputPassword, key);
            return hash == hashedPassword;
        }
    }
}
