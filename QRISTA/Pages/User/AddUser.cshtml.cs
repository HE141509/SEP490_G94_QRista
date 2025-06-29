
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Data.SqlClient;
using System;

namespace QRB.Pages.User
{
    [IgnoreAntiforgeryToken]
    public class AddUserModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }
                var json = JsonDocument.Parse(body);
                var root = json.RootElement;

                string tenHienThi = root.GetProperty("TenHienThi").GetString();
                string tenNguoiDung = root.GetProperty("TenNguoiDung").GetString();
                string matKhau = root.GetProperty("MatKhau").GetString();
                string trangThai = root.TryGetProperty("TrangThai", out var tt) ? tt.GetString() : "active";
                string idChiNhanhStr = root.TryGetProperty("IDChiNhanh", out var cn) ? cn.GetString() : null;
                if (string.IsNullOrWhiteSpace(tenHienThi) || string.IsNullOrWhiteSpace(tenNguoiDung) || string.IsNullOrWhiteSpace(matKhau) || string.IsNullOrWhiteSpace(idChiNhanhStr))
                {
                    return new JsonResult(new { success = false, message = "Thiếu thông tin bắt buộc." });
                }
                if (!Guid.TryParse(idChiNhanhStr, out Guid idChiNhanh))
                {
                    return new JsonResult(new { success = false, message = "Chi nhánh không hợp lệ." });
                }

                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    // Kiểm tra trùng tên đăng nhập
                    var checkCmd = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE TenNguoiDung = @TenNguoiDung AND (IsDelete = 0 OR IsDelete IS NULL)", connection);
                    checkCmd.Parameters.AddWithValue("@TenNguoiDung", tenNguoiDung);
                    int count = (int)await checkCmd.ExecuteScalarAsync();
                    if (count > 0)
                    {
                        return new JsonResult(new { success = false, message = "Tên đăng nhập đã tồn tại." });
                    }

                    var insertCmd = new SqlCommand(@"INSERT INTO NguoiDung (ID, TenHienThi, TenNguoiDung, MatKhau, IsDelete, IDChiNhanh, CreateTime) VALUES (@ID, @TenHienThi, @TenNguoiDung, @MatKhau, @IsDelete, @IDChiNhanh, @CreateTime)", connection);
                    Guid newId = Guid.NewGuid();
                    insertCmd.Parameters.AddWithValue("@ID", newId);
                    insertCmd.Parameters.AddWithValue("@TenHienThi", tenHienThi);
                    insertCmd.Parameters.AddWithValue("@TenNguoiDung", tenNguoiDung);
                    insertCmd.Parameters.AddWithValue("@MatKhau", matKhau); // Nên mã hóa mật khẩu ở đây
                    insertCmd.Parameters.AddWithValue("@IsDelete", trangThai == "active" ? 0 : 1);
                    insertCmd.Parameters.AddWithValue("@IDChiNhanh", idChiNhanh);
                    insertCmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);

                    int rows = await insertCmd.ExecuteNonQueryAsync();
                    if (rows > 0)
                    {
                        return new JsonResult(new { success = true, message = "Thêm mới thành công!" });
                    }
                }
                return new JsonResult(new { success = false, message = "Không thể thêm mới user." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
