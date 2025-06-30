using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.Json;

namespace QRB.Pages.Branch
{
    public class DeleteBranchModel : PageModel
    {
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

                    string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
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
