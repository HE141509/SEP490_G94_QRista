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
public class UpdateCustomerModel : PageModel
{
    private readonly IConfiguration _configuration;
    public UpdateCustomerModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public class UpdateCustomerInput
    {
        public Guid Id { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string? GiaTriDonHang { get; set; }
        public bool IsDelete { get; set; }
    }
    public IActionResult OnPost()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return new JsonResult(new { success = false, message = "Chưa đăng nhập." });

        UpdateCustomerInput? input = null;
        using (var reader = new StreamReader(Request.Body))
        {
            var body = reader.ReadToEndAsync().Result;
            input = JsonSerializer.Deserialize<UpdateCustomerInput>(body);
        }
        if (input == null || input.Id == Guid.Empty || string.IsNullOrWhiteSpace(input.TenKhachHang) || string.IsNullOrWhiteSpace(input.SoDienThoai))
        {
            return new JsonResult(new { success = false, message = "Thiếu thông tin khách hàng." });
        }
        string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string sql = "UPDATE KhachHang SET TenKhachHang=@TenKhachHang, SDT=@SDT, GiaTriDonHang=@GiaTriDonHang, IsDelete=@IsDelete, UpdateTime=@UpdateTime WHERE ID=@ID";
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ID", input.Id);
                command.Parameters.AddWithValue("@TenKhachHang", input.TenKhachHang.Trim());
                command.Parameters.AddWithValue("@SDT", input.SoDienThoai.Trim());
                command.Parameters.AddWithValue("@GiaTriDonHang", (object?)input.GiaTriDonHang ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsDelete", input.IsDelete);
                command.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = "Lỗi khi cập nhật khách hàng: " + ex.Message });
                }
            }
        }
        return new JsonResult(new { success = true });
    }
}
}
