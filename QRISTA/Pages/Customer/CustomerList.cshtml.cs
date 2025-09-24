using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using QRB.Models;
using System.Text.Json;

namespace QRB.Pages.Customer
{
    public class CustomerListModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<CustomerInfo> Customers { get; set; } = new List<CustomerInfo>();
        public string UserName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;

        public CustomerListModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private bool HasPermission(string permissionName)
        {
            var permissionsJson = HttpContext.Session.GetString("UserPermissions");
            if (string.IsNullOrEmpty(permissionsJson))
            {
                return false;
            }
            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
                return permissions?.Contains(permissionName) ?? false;
            }
            catch
            {
                return false;
            }
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return Redirect("/Index");
            }

            // Kiểm tra quyền truy cập
            if (!HasPermission("Full Customers"))
            {
                return Redirect($"/AccessDenied?permission=Full Customers&module=Customers");
            }

            UserName = HttpContext.Session.GetString("Username") ?? string.Empty;
            UserAvatar = HttpContext.Session.GetString("UserAvatar") ?? string.Empty;
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT ID, CustomerName, Phone, GiaTriDonHang, IsDelete, CreateTime, UpdateTime FROM Customer ORDER BY CustomerName";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        int stt = 1;
                        while (reader.Read())
                        {
                            var c = new CustomerInfo();
                            c.STT = stt++;
                            c.Id = reader.GetGuid(0);
                            c.TenKhachHang = reader.GetString(1);  // CustomerName
                            c.SoDienThoai = reader.GetString(2);   // Phone
                            c.GiaTriDonHang = reader.IsDBNull(3) ? null : reader.GetString(3);
                            c.IsDelete = reader.IsDBNull(4) ? false : reader.GetBoolean(4);
                            c.CreateTime = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
                            c.UpdateTime = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                            Customers.Add(c);
                        }
                    }
                }
            }
            return Page();
        }
    }
}
