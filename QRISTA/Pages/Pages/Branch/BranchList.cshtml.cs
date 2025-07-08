using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace QRB.Pages.Branch
{
    public class BranchListModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public BranchListModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public class BranchInfo
        {
            public Guid ID { get; set; }
            public string MaChiNhanh { get; set; } = string.Empty;
            public string TenChiNhanh { get; set; } = string.Empty;
            public bool IsDelete { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public List<BranchInfo> Branches { get; set; } = new List<BranchInfo>();
        public string? CurrentUserDisplayName { get; set; }

        public IActionResult OnGet()
        {
            // Kiểm tra session đăng nhập
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Index");
            }
            CurrentUserDisplayName = HttpContext.Session.GetString("DisplayName");

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"SELECT ID, MaChiNhanh, TenChiNhanh, ISNULL(IsDelete,0), ISNULL(CreateTime, GETDATE()) FROM ChiNhanh", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Branches.Add(new BranchInfo
                        {
                            ID = reader.GetGuid(0),
                            MaChiNhanh = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            TenChiNhanh = reader.GetString(2),
                            IsDelete = reader.GetBoolean(3),
                            CreateTime = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return Page();
        }
    }
}
