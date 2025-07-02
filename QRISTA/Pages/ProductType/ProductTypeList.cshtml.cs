using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace QRB.Pages.ProductType
{
    [IgnoreAntiforgeryToken]
    public class ProductTypeListModel : PageModel
    {
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

        public class LoaiSanPhamViewModel
        {
            public Guid ID { get; set; }
            public Guid IDSanPham { get; set; }
            public string TenLoai { get; set; } = string.Empty;
            public string MaLoai { get; set; } = string.Empty;
            public string DonGia { get; set; } = string.Empty;
            public bool IsDelete { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime? UpdateTime { get; set; }
            public Guid IDChiNhanh { get; set; }
            public string TenChiNhanh { get; set; } = string.Empty;
            public string TenSanPham { get; set; } = string.Empty;
        }

        public class SanPhamItem
        {
            public Guid ID { get; set; }
            public string TenSanPham { get; set; } = string.Empty;
        }
        public class ChiNhanhItem
        {
            public Guid ID { get; set; }
            public string TenChiNhanh { get; set; } = string.Empty;
        }

        public List<LoaiSanPhamViewModel> LoaiSanPhams { get; set; } = new();
        public List<SanPhamItem> SanPhams { get; set; } = new();
        public List<ChiNhanhItem> ChiNhanhs { get; set; } = new();



        public void OnGet()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Lấy loại sản phẩm
                var command = new SqlCommand(@"
                    SELECT lsp.ID, lsp.IDSanPham, lsp.TenLoai, lsp.MaLoai, lsp.DonGia, lsp.IsDelete, lsp.CreateTime, lsp.UpdateTime, lsp.IDChiNhanh,
                           ISNULL(cn.TenChiNhanh, N''), ISNULL(sp.TenSanPham, N'')
                    FROM LoaiSanPham lsp
                    LEFT JOIN ChiNhanh cn ON lsp.IDChiNhanh = cn.ID
                    LEFT JOIN SanPham sp ON lsp.IDSanPham = sp.ID
                ", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LoaiSanPhams.Add(new LoaiSanPhamViewModel
                        {
                            ID = reader.GetGuid(0),
                            IDSanPham = reader.GetGuid(1),
                            TenLoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            MaLoai = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            DonGia = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            IsDelete = reader.IsDBNull(5) ? false : reader.GetBoolean(5),
                            CreateTime = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6),
                            UpdateTime = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                            IDChiNhanh = reader.GetGuid(8),
                            TenChiNhanh = reader.IsDBNull(9) ? "" : reader.GetString(9),
                            TenSanPham = reader.IsDBNull(10) ? "" : reader.GetString(10)
                        });
                    }
                }
                // Lấy danh sách sản phẩm
                var cmdSP = new SqlCommand("SELECT ID, TenSanPham FROM SanPham WHERE IsDelete=0", connection);
                using (var reader = cmdSP.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SanPhams.Add(new SanPhamItem
                        {
                            ID = reader.GetGuid(0),
                            TenSanPham = reader.IsDBNull(1) ? "" : reader.GetString(1)
                        });
                    }
                }
                // Lấy danh sách chi nhánh
                var cmdCN = new SqlCommand("SELECT ID, TenChiNhanh FROM ChiNhanh WHERE IsDelete=0", connection);
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
        }

        public JsonResult OnGetSanPhamByChiNhanh(Guid chiNhanhId)
        {
            try
            {
                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"SELECT ID, TenSanPham FROM SanPham WHERE IDChiNhanh = @IDChiNhanh", connection);
                    command.Parameters.AddWithValue("@IDChiNhanh", chiNhanhId);

                    var reader = command.ExecuteReader();
                    var sanPhams = new List<SanPhamItem>();
                    while (reader.Read())
                    {
                        sanPhams.Add(new SanPhamItem
                        {
                            ID = reader.GetGuid(0),
                            TenSanPham = reader.GetString(1)
                        });
                    }
                    return new JsonResult(sanPhams);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}

