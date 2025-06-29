using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace QRB.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public string? PhoneNumber { get; set; }
    public bool Skipped { get; set; }
    public int? TableNumber { get; set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(int? table)
    {
        TableNumber = table;
        if (table.HasValue)
        {
            HttpContext.Session.SetInt32("TableNumber", table.Value);
        }
    }

    public IActionResult OnPost(string? skip)
    {
        if (!string.IsNullOrEmpty(skip))
        {
            Skipped = true;
            // Xử lý khi người dùng bấm Bỏ qua
            return RedirectToPage("/Menu/Menu"); // hoặc trang tiếp theo
        }
        if (!string.IsNullOrEmpty(PhoneNumber))
        {
            // Xử lý số điện thoại, ví dụ lưu session hoặc chuyển trang
            return RedirectToPage("/Menu/Menu"); // hoặc trang tiếp theo
        }
        return Page();
    }
}
