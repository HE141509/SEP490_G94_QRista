
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
                        var cmd = new SqlCommand("UPDATE ChiNhanh SET MaChiNhanh = @MaChiNhanh, TenChiNhanh = @TenChiNhanh, IsDelete = @IsDelete WHERE ID = @ID", connection);
                        cmd.Parameters.AddWithValue("@ID", input.ID);
                        cmd.Parameters.AddWithValue("@MaChiNhanh", input.MaChiNhanh.Trim());
                        cmd.Parameters.AddWithValue("@TenChiNhanh", input.TenChiNhanh.Trim());
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
