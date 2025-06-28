using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QRB.Pages
{
    public class DashboardModel : PageModel
    {
        public string UserName { get; private set; } = string.Empty;
        public bool IsLoggedIn { get; private set; }

        public IActionResult OnGet()
        {
            // Kiểm tra session đăng nhập
            var userId = HttpContext.Session.GetString("UserId");
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                // Chưa đăng nhập, chuyển về trang login
                return RedirectToPage("/Login");
            }

            IsLoggedIn = true;
            UserName = HttpContext.Session.GetString("DisplayName") ?? username;

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            // Xóa session
            HttpContext.Session.Clear();
            
            // Xóa cookie remember me nếu có
            if (Request.Cookies.ContainsKey("RememberMe"))
            {
                Response.Cookies.Delete("RememberMe");
            }

            return RedirectToPage("/Login");
        }
    }
}
