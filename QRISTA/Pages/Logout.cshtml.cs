using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QRB.Pages
{
    public class LogoutModel : PageModel
    {
        //kiem tra cac thu
        public IActionResult OnGet()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Clear remember me cookie
            if (Request.Cookies.ContainsKey("RememberMe"))
            {
                Response.Cookies.Delete("RememberMe");
            }

            // Redirect to menu page
            return RedirectToPage("/Menu");
        }
    }
}
