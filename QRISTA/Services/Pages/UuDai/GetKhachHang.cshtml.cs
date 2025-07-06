using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace QRB.Pages.UuDai
{
    [IgnoreAntiforgeryToken]
    public class GetKhachHangModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public GetKhachHangModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JsonResult OnGet(string search = "")
        {
            var result = new List<object>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT TOP 20 Id, TenKhachHang, SDT FROM KhachHang WHERE IsDelete = 0 AND (TenKhachHang LIKE @search OR SDT LIKE @search) ORDER BY CreateTime DESC", conn);
                cmd.Parameters.AddWithValue("@search", "%" + (search ?? "") + "%");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new {
                            ID = reader["Id"] == DBNull.Value ? string.Empty : reader["Id"].ToString(),
                            TenKhachHang = reader["TenKhachHang"] == DBNull.Value ? string.Empty : reader["TenKhachHang"].ToString(),
                            SoDienThoai = reader["SDT"] == DBNull.Value ? string.Empty : reader["SDT"].ToString()
                        });
                    }
                }
            }
            return new JsonResult(result);
        }
    }
}
