using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace QRB.Pages.User
{
    public class DeleteUserModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                string body;
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }
                var json = System.Text.Json.JsonDocument.Parse(body);
                var idProp = json.RootElement.GetProperty("id");
                Guid id = idProp.GetGuid();

                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("UPDATE NguoiDung SET IsDelete = 1 WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", id);
                    int rows = await command.ExecuteNonQueryAsync();
                    if (rows > 0)
                    {
                        return new JsonResult(new { success = true });
                    }
                }
                return new JsonResult(new { success = false });
            }
            catch
            {
                return new JsonResult(new { success = false });
            }
        }
    }
}
