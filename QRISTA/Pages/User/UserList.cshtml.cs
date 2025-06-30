using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace QRB.Pages.User
{
    public class UserListModel : PageModel
    {
        public class UserInfo
        {
            public Guid ID { get; set; }
            public string TenHienThi { get; set; }
            public string TenNguoiDung { get; set; }
            public string TenChiNhanh { get; set; }
            public bool IsDelete { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public List<UserInfo> Users { get; set; } = new List<UserInfo>();
        public class ChiNhanhInfo
        {
            public Guid ID { get; set; }
            public string TenChiNhanh { get; set; } = string.Empty;
        }

        public List<ChiNhanhInfo> ChiNhanhs { get; set; } = new List<ChiNhanhInfo>();

        public string? CurrentUserDisplayName { get; set; }

        public IActionResult OnGet()
        {
            // Kiểm tra session đăng nhập
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                // Chưa đăng nhập, chuyển về trang index
                return Redirect("/Index");
            }

            // Lấy tên hiển thị user hiện tại từ session
            CurrentUserDisplayName = HttpContext.Session.GetString("DisplayName");

            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Lấy danh sách user
                var command = new SqlCommand(@"SELECT u.ID, u.TenHienThi, u.TenNguoiDung, c.TenChiNhanh, ISNULL(u.IsDelete,0), u.CreateTime
FROM NguoiDung u
LEFT JOIN ChiNhanh c ON u.IDChiNhanh = c.ID
ORDER BY u.CreateTime DESC", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users.Add(new UserInfo
                        {
                            ID = reader.GetGuid(0),
                            TenHienThi = reader.GetString(1),
                            TenNguoiDung = reader.GetString(2),
                            TenChiNhanh = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            IsDelete = reader.GetBoolean(4),
                            CreateTime = reader.GetDateTime(5)
                        });
                    }
                }
                // Lấy danh sách chi nhánh
                var cnCommand = new SqlCommand(@"SELECT ID, TenChiNhanh FROM ChiNhanh WHERE IsDelete = 0 OR IsDelete IS NULL", connection);
                using (var reader = cnCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ChiNhanhs.Add(new ChiNhanhInfo
                        {
                            ID = reader.GetGuid(0),
                            TenChiNhanh = reader.GetString(1)
                        });
                    }
                }
            }
            return Page();
        }
    }
}
