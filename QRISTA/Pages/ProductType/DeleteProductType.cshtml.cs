using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.Json;

namespace QRB.Pages.ProductType
{
    public class DeleteProductTypeModel : PageModel
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
                var input = JsonSerializer.Deserialize<DeleteRequest>(body);
                if (input == null || input.id == Guid.Empty)
                    return new JsonResult(new { success = false, message = "ID không hợp lệ!" });

                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("UPDATE LoaiSanPham SET IsDelete=1 WHERE ID=@id", connection);
                    command.Parameters.AddWithValue("@id", input.id);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                        return new JsonResult(new { success = true });
                    else
                        return new JsonResult(new { success = false, message = $"Không tìm thấy loại sản phẩm với ID: {input.id}" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        public class DeleteRequest
        {
            public Guid id { get; set; }
        }
    }
}
