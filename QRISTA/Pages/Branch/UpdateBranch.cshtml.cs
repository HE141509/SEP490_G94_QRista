
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace QRB.Pages.Branch
{
    [IgnoreAntiforgeryToken]
    public class UpdateBranchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public UpdateBranchModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public class BranchInput
        {
            public Guid ID { get; set; }
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
                    if (input == null || input.ID == Guid.Empty || string.IsNullOrWhiteSpace(input.TenChiNhanh) || string.IsNullOrWhiteSpace(input.MaChiNhanh))
                        return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });

                    var connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        
                        // Kiểm tra mã chi nhánh đã tồn tại chưa (trừ chính nó)
                        var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Department WHERE DepartmentCode = @DepartmentCode AND ID != @ID", connection);
                        checkCmd.Parameters.AddWithValue("@DepartmentCode", input.MaChiNhanh.Trim());
                        checkCmd.Parameters.AddWithValue("@ID", input.ID);
                        var count = (int)checkCmd.ExecuteScalar();
                        
                        if (count > 0)
                        {
                            return new JsonResult(new { success = false, message = "Mã chi nhánh đã tồn tại!" });
                        }
                        
                        var cmd = new SqlCommand("UPDATE Department SET DepartmentCode = @DepartmentCode, DepartmentName = @DepartmentName, IsDelete = @IsDelete WHERE ID = @ID", connection);
                        cmd.Parameters.AddWithValue("@ID", input.ID);
                        cmd.Parameters.AddWithValue("@DepartmentCode", input.MaChiNhanh.Trim());
                        cmd.Parameters.AddWithValue("@DepartmentName", input.TenChiNhanh.Trim());
                        cmd.Parameters.AddWithValue("@IsDelete", input.IsDelete);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                            return new JsonResult(new { success = true });
                        else
                            return new JsonResult(new { success = false, message = "Không tìm thấy chi nhánh để cập nhật!" });
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
