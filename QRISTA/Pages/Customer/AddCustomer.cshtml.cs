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
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
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
        if (input == null || string.IsNullOrWhiteSpace(input.CustomerName) || string.IsNullOrWhiteSpace(input.Phone))
        {
            return new JsonResult(new { success = false, message = "Tên khách hàng và SĐT là bắt buộc." });
        }
        string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string sql = "INSERT INTO Customer (ID, CustomerName, Phone, GiaTriDonHang, IsDelete, CreateTime, UpdateTime) VALUES (@ID, @CustomerName, @Phone, @GiaTriDonHang, @IsDelete, @CreateTime, @UpdateTime)";
            using (var command = new SqlCommand(sql, connection))
            {
                var newId = Guid.NewGuid();
                command.Parameters.AddWithValue("@ID", newId);
                command.Parameters.AddWithValue("@CustomerName", input.CustomerName.Trim());
                command.Parameters.AddWithValue("@Phone", input.Phone.Trim());
                command.Parameters.AddWithValue("@GiaTriDonHang", (object?)input.GiaTriDonHang ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsDelete", input.IsDelete);
                var now = DateTime.Now;
                command.Parameters.AddWithValue("@CreateTime", now);
                command.Parameters.AddWithValue("@UpdateTime", now);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                    {
                        string message;
                        if (ex.Message.Contains("UQ_Customer_Phone")) // constraint name
                        {
                            message = "Số điện thoại đã tồn tại";
                        }
                        else
                        {
                            message = "Có lỗi xảy ra: " + ex.Message;
                        }

                        return new JsonResult(new { success = false, message });
                    }
            }
        }
        return new JsonResult(new { success = true });
    }
}
}
