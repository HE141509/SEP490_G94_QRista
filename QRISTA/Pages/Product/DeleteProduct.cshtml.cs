using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace QRB.Pages.Product
{
    public class DeleteProductModel : PageModel
    {
        [IgnoreAntiforgeryToken]
        public IActionResult OnPost()
        {
            try
            {
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = reader.ReadToEndAsync().Result;
                }
                var req = System.Text.Json.JsonSerializer.Deserialize<DeleteRequest>(body);
                if (req == null || req.id == Guid.Empty)
                    return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ." });

                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("UPDATE SanPham SET IsDelete=1 WHERE ID=@id", connection);
                    command.Parameters.AddWithValue("@id", req.id);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                        return new JsonResult(new { success = true });
                    else
                        return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public class DeleteRequest
        {
            public Guid id { get; set; }
        }
    }
}
