using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace QRB.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _configuration;

    [BindProperty]
    public string? PhoneNumber { get; set; }
    public bool Skipped { get; set; }
    public int? TableNumber { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void OnGet(int? table)
    {
        TableNumber = table;
        if (table.HasValue)
        {
            HttpContext.Session.SetInt32("TableNumber", table.Value);
        }
    }

    // Duplicate _configuration and constructor removed

    public string? ErrorMessage { get; set; }

    public IActionResult OnPost(string? skip)
    {
        if (!string.IsNullOrEmpty(skip))
        {
            Skipped = true;
            return RedirectToPage("/Menu/Menu");
        }
        if (!string.IsNullOrEmpty(PhoneNumber))
        {
            // Kiểm tra số điện thoại trong bảng KhachHang
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(1) FROM KhachHang WHERE SDT = @SDT AND IsDelete = 0";
                using (var command = new Microsoft.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SDT", PhoneNumber.Trim());
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        // Đã đăng ký, cho vào menu
                        HttpContext.Session.SetString("PhoneNumber", PhoneNumber.Trim());
                        return RedirectToPage("/Menu/Menu");
                    }
                    else
                    {
                        ErrorMessage = "Số điện thoại chưa đăng ký!";
                        ModelState.AddModelError("PhoneNumber", ErrorMessage);
                        return Page();
                    }
                }
            }
        }
        return Page();
    }
}
