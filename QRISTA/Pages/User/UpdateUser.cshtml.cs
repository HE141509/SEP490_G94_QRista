using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace QRB.Pages.User
{
    [IgnoreAntiforgeryToken]
    public class UpdateUserModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public UpdateUserModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class UpdateUserRequest
        {
            public string ID { get; set; }
            public string TenHienThi { get; set; }
            public string TenNguoiDung { get; set; }
            public string MatKhau { get; set; } // Có thể rỗng nếu không đổi
            public string IDChiNhanh { get; set; }
            public string TrangThai { get; set; } // "active" hoặc "inactive"
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var req = JsonSerializer.Deserialize<UpdateUserRequest>(body);
            if (req == null || string.IsNullOrEmpty(req.ID))
                return new JsonResult(new { success = false, message = "Thiếu thông tin user" });

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var tran = conn.BeginTransaction();
                try
                {
                    var sql = new StringBuilder();
                    sql.Append("UPDATE NguoiDung SET TenHienThi = @TenHienThi, IDChiNhanh = @IDChiNhanh, IsDelete = @IsDelete");
                    if (!string.IsNullOrEmpty(req.MatKhau))
                    {
                        sql.Append(", MatKhau = @MatKhau");
                    }
                    sql.Append(" WHERE ID = @ID");
                    var cmd = new SqlCommand(sql.ToString(), conn, tran);
                    cmd.Parameters.AddWithValue("@ID", req.ID);
                    cmd.Parameters.AddWithValue("@TenHienThi", req.TenHienThi ?? "");
                    cmd.Parameters.AddWithValue("@IDChiNhanh", req.IDChiNhanh ?? "");
                    cmd.Parameters.AddWithValue("@IsDelete", req.TrangThai == "inactive" ? 1 : 0);
                    if (!string.IsNullOrEmpty(req.MatKhau))
                    {
                        // Mã hóa MD5 + key từ appsettings
                        var key = _configuration["PasswordKey"] ?? "qrb2025";
                        string hash = GetMd5Hash(req.MatKhau + key);
                        cmd.Parameters.AddWithValue("@MatKhau", hash);
                    }
                    int rows = await cmd.ExecuteNonQueryAsync();
                    tran.Commit();
                    if (rows > 0)
                        return new JsonResult(new { success = true });
                    else
                        return new JsonResult(new { success = false, message = "Không tìm thấy user để cập nhật" });
                }
                catch (System.Exception ex)
                {
                    tran.Rollback();
                    return new JsonResult(new { success = false, message = ex.Message });
                }
            }
        }

        private string GetMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
