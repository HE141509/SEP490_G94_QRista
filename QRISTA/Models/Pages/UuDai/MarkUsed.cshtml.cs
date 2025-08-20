using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace QRB.Pages.UuDai
{
    public class MarkUsedModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public MarkUsedModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public JsonResult OnPost([FromBody] MarkUsedRequest req)
        {
            if (req == null || req.id == Guid.Empty)
                return new JsonResult(new { success = false, message = "ID không hợp lệ!" });
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE MaUuDai SET TrangThaiSuDung = 1, UpdateTime = GETDATE() WHERE ID = @ID";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", req.id);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                        return new JsonResult(new { success = true });
                    else
                        return new JsonResult(new { success = false, message = "Không tìm thấy mã ưu đãi!" });
                }
            }
        }
        public class MarkUsedRequest
        {
            public Guid id { get; set; }
        }
    }
}
