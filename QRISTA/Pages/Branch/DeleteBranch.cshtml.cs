using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace QRB.Pages.Branch
{
    public class DeleteBranchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public DeleteBranchModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public class DeleteInput
        {
            public Guid id { get; set; }
        }

        public IActionResult OnPost()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = reader.ReadToEndAsync().Result;
                    var input = JsonSerializer.Deserialize<DeleteInput>(body);
                    if (input == null || input.id == Guid.Empty)
                        return new JsonResult(new { success = false, message = "ID không hợp lệ!" });

                    var connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        var cmd = new SqlCommand("UPDATE ChiNhanh SET IsDelete = 1 WHERE ID = @ID", connection);
                        cmd.Parameters.AddWithValue("@ID", input.id);
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
            return new JsonResult(new { success = false, message = "Xóa chi nhánh thất bại!" });
        }
    }
}
