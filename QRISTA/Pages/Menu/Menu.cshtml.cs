using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QRB.Pages.Menu;

public class MenuModel : PageModel
{
    public bool IsLoggedIn { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ChiNhanhName { get; set; } = string.Empty;

    public void OnGet()
    {
        // Kiểm tra trạng thái đăng nhập
        IsLoggedIn = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        
        if (IsLoggedIn)
        {
            DisplayName = HttpContext.Session.GetString("DisplayName") ?? "Người dùng";
            ChiNhanhName = HttpContext.Session.GetString("ChiNhanhName") ?? "Chi nhánh";
        }
    }
}
