using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.Json;

namespace QRB.Pages.Branch
{
    public class AddBranchModel : PageModel
    {
        public class BranchInput
        {
            public string MaChiNhanh { get; set; } = string.Empty;
            public string TenChiNhanh { get; set; } = string.Empty;
            public bool IsDelete { get; set; } = false;
        }

        public IActionResult OnPost()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = reader.ReadToEndAsync().Result;
                    var input = JsonSerializer.Deserialize<BranchInput>(body);
                    if (input == null || string.IsNullOrWhiteSpace(input.TenChiNhanh) || string.IsNullOrWhiteSpace(input.MaChiNhanh))
                        return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });

                    string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var cmd = new SqlCommand("INSERT INTO ChiNhanh (ID, MaChiNhanh, TenChiNhanh, IsDelete, CreateTime) VALUES (@ID, @MaChiNhanh, @TenChiNhanh, @IsDelete, @CreateTime)", connection);
                        cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@MaChiNhanh", input.MaChiNhanh.Trim());
                        cmd.Parameters.AddWithValue("@TenChiNhanh", input.TenChiNhanh.Trim());
                        cmd.Parameters.AddWithValue("@IsDelete", input.IsDelete);
                        cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                            return new JsonResult(new { success = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
            return new JsonResult(new { success = false, message = "Thêm chi nhánh thất bại!" });
        }
    }
}
