using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

using Microsoft.Extensions.Configuration;
using QRB.Models;

namespace QRB.Pages.Customer
{
using System.Text.Json;
[IgnoreAntiforgeryToken]
public class AddCustomerModel : PageModel
{
    private readonly IConfiguration _configuration;
    public AddCustomerModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public class AddCustomerInput
    {
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string? GiaTriDonHang { get; set; }
        public bool IsDelete { get; set; }
    }
    public IActionResult OnPost()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return new JsonResult(new { success = false, message = "Chưa đăng nhập." });

        AddCustomerInput? input = null;
        using (var reader = new StreamReader(Request.Body))
        {
            var body = reader.ReadToEndAsync().Result;
            input = JsonSerializer.Deserialize<AddCustomerInput>(body);
        }
        if (input == null || string.IsNullOrWhiteSpace(input.TenKhachHang) || string.IsNullOrWhiteSpace(input.SoDienThoai))
        {
            return new JsonResult(new { success = false, message = "Tên khách hàng và SĐT là bắt buộc." });
        }
        string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string sql = "INSERT INTO KhachHang (ID, TenKhachHang, SDT, GiaTriDonHang, IsDelete, CreateTime, UpdateTime) VALUES (@ID, @TenKhachHang, @SDT, @GiaTriDonHang, @IsDelete, @CreateTime, @UpdateTime)";
            using (var command = new SqlCommand(sql, connection))
            {
                var newId = Guid.NewGuid();
                command.Parameters.AddWithValue("@ID", newId);
                command.Parameters.AddWithValue("@TenKhachHang", input.TenKhachHang.Trim());
                command.Parameters.AddWithValue("@SDT", input.SoDienThoai.Trim());
                command.Parameters.AddWithValue("@GiaTriDonHang", (object?)input.GiaTriDonHang ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsDelete", input.IsDelete);
                var now = DateTime.Now;
                command.Parameters.AddWithValue("@CreateTime", now);
                command.Parameters.AddWithValue("@UpdateTime", now);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = "Lỗi khi thêm khách hàng: " + ex.Message });
                }
            }
        }
        return new JsonResult(new { success = true });
    }
}
}
