using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace QRB.Pages.User
{
    public class GetUserByIdModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public GetUserByIdModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new JsonResult(new { success = false, message = "Thiếu ID" });

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                // Đổi tên bảng thành NguoiDung
                var cmd = new SqlCommand("SELECT TOP 1 ID, TenHienThi, TenNguoiDung, IDChiNhanh, IsDelete FROM NguoiDung WHERE ID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new JsonResult(new
                        {
                            id = reader["ID"].ToString(),
                            tenHienThi = reader["TenHienThi"].ToString(),
                            tenNguoiDung = reader["TenNguoiDung"].ToString(),
                            idChiNhanh = reader["IDChiNhanh"].ToString(),
                            isDelete = Convert.ToBoolean(reader["IsDelete"])
                        });
                    }
                }
            }
            return new JsonResult(new { success = false, message = "Không tìm thấy user" });
        }
    }
}
