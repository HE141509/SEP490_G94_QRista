using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

using Microsoft.Extensions.Configuration;

namespace QRB.Pages.Customer
{
using System.Text.Json;
[IgnoreAntiforgeryToken]
public class DeleteCustomerModel : PageModel
{
    private readonly IConfiguration _configuration;
    public DeleteCustomerModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public class DeleteInput
    {
        public Guid id { get; set; }
    }
    public IActionResult OnPost()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return new JsonResult(new { success = false, message = "Chưa đăng nhập." });

        DeleteInput? input = null;
        using (var reader = new StreamReader(Request.Body))
        {
            var body = reader.ReadToEndAsync().Result;
            input = JsonSerializer.Deserialize<DeleteInput>(body);
        }
        if (input == null || input.id == Guid.Empty)
        {
            return new JsonResult(new { success = false, message = "Thiếu ID khách hàng." });
        }
        string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string sql = "UPDATE KhachHang SET IsDelete=@IsDelete, UpdateTime=@UpdateTime WHERE ID=@Id";
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", input.id);
                command.Parameters.AddWithValue("@IsDelete", true);
                command.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = "Lỗi khi xóa khách hàng: " + ex.Message });
                }
            }
        }
        return new JsonResult(new { success = true });
    }
}
}
