using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Text.Json;

namespace QRB.Pages.Product
{
    public class ProductListModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public ProductListModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            var username = context.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToPageResult("/Login");
            }
            base.OnPageHandlerExecuting(context);
        }
        private bool HasPermission(string permissionName)
        {
            var permissionsJson = HttpContext.Session.GetString("UserPermissions");
            if (string.IsNullOrEmpty(permissionsJson))
            {
                return false;
            }
            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
                return permissions?.Contains(permissionName) ?? false;
            }
            catch
            {
                return false;
            }
        }

        public class SanPhamViewModel
        {
            public Guid ID { get; set; }
            public string? MaSanPham { get; set; }
            public string? TenSanPham { get; set; }
            public byte[]? HinhAnh { get; set; }
            public string? NoiDung { get; set; }
            public Guid IdCategory { get; set; }
            public string? CategoryName { get; set; }
            public Guid IDChiNhanh { get; set; }
            public bool IsDelete { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime? UpdateTime { get; set; }
            public string? TenChiNhanh { get; set; }
        }

        public List<SanPhamViewModel> SanPhams { get; set; } = new();
    public List<CategoryItem> Categories { get; set; } = new();
        public List<ChiNhanhItem> ChiNhanhs { get; set; } = new();

        public class CategoryItem
        {
            public Guid ID { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }
        public class ChiNhanhItem
        {
            public Guid ID { get; set; }
            public string TenChiNhanh { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Index");
            }

            if (!HasPermission("Full Products"))
            {
                return Redirect($"/AccessDenied?permission=Full Products&module=Products");
            }

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT sp.ID, sp.ProductCode, sp.ProductName, sp.Picture, sp.NoiDung,
                           sp.IdCategory, sp.IDDepartment,
                           ISNULL(nsp.CategoryName, N''), ISNULL(d.DepartmentName, N''), 
                           ISNULL(sp.IsDelete,0), sp.CreateTime, sp.UpdateTime
                    FROM Product sp
                    LEFT JOIN Category nsp ON sp.IdCategory = nsp.ID
                    LEFT JOIN Department d ON sp.IDDepartment = d.ID
                ", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanPhams.Add(new SanPhamViewModel
                        {
                            ID = reader.GetGuid(0),
                            MaSanPham = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            TenSanPham = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            HinhAnh = reader.IsDBNull(3) ? null : (byte[])reader[3],
                            NoiDung = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            IdCategory = reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5),
                            IDChiNhanh = reader.IsDBNull(6) ? Guid.Empty : reader.GetGuid(6),
                            CategoryName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            TenChiNhanh = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            IsDelete = reader.IsDBNull(9) ? false : reader.GetBoolean(9),
                            CreateTime = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10),
                            UpdateTime = reader.IsDBNull(11) ? null : reader.GetDateTime(11)
                        });
                    }
                }
                // Lấy danh sách nhóm sản phẩm
                var cmdNhom = new SqlCommand("SELECT ID, CategoryName FROM Category WHERE IsDelete=0", connection);
                using (var reader = cmdNhom.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Categories.Add(new CategoryItem
                        {
                            ID = reader.GetGuid(0),
                            CategoryName = reader.IsDBNull(1) ? "" : reader.GetString(1)
                        });
                    }
                }
                // Lấy danh sách chi nhánh
                var cmdCN = new SqlCommand("SELECT ID, DepartmentName FROM Department WHERE IsDelete=0", connection);
                using (var reader = cmdCN.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ChiNhanhs.Add(new ChiNhanhItem
                        {
                            ID = reader.GetGuid(0),
                            TenChiNhanh = reader.IsDBNull(1) ? "" : reader.GetString(1)
                        });
                    }
                }
            }
            return Page();
        }
        // ...
    }
}
