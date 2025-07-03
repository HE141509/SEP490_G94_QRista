using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QRB.Pages.Branch
{
    public class GetBranchByIdModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public GetBranchByIdModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult OnGet(Guid id)
        {
            if (id == Guid.Empty) return new JsonResult(null);
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand("SELECT ID, MaChiNhanh, TenChiNhanh, ISNULL(IsDelete,0), ISNULL(CreateTime, GETDATE()) FROM ChiNhanh WHERE ID = @ID", connection);
                cmd.Parameters.AddWithValue("@ID", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new JsonResult(new {
                            id = reader.GetGuid(0),
                            maChiNhanh = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            tenChiNhanh = reader.GetString(2),
                            isDelete = reader.GetBoolean(3),
                            createTime = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return new JsonResult(null);
        }
    }
}
