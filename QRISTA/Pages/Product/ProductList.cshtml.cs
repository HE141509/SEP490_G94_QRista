using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;

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

        public class SanPhamViewModel
        {
            public Guid ID { get; set; }
            public string? MaSanPham { get; set; }
            public string? TenSanPham { get; set; }
            public byte[]? HinhAnh { get; set; }
            public Guid IdNhomSanPham { get; set; }
            public string? TenNhom { get; set; }
            public Guid IDChiNhanh { get; set; }
            public bool IsDelete { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime? UpdateTime { get; set; }
            public string? TenChiNhanh { get; set; }
        }

        public List<SanPhamViewModel> SanPhams { get; set; } = new();
        public List<NhomSanPhamItem> NhomSanPhams { get; set; } = new();
        public List<ChiNhanhItem> ChiNhanhs { get; set; } = new();

        public class NhomSanPhamItem
        {
            public Guid ID { get; set; }
            public string TenNhom { get; set; } = string.Empty;
        }
        public class ChiNhanhItem
        {
            public Guid ID { get; set; }
            public string TenChiNhanh { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT sp.ID, sp.MaSanPham, sp.TenSanPham, sp.HinhAnh,
                           sp.IdNhomSanPham, sp.IDChiNhanh,
                           ISNULL(nsp.TenNhom, N''), ISNULL(cn.TenChiNhanh, N''), 
                           ISNULL(sp.IsDelete,0), sp.CreateTime, sp.UpdateTime
                    FROM SanPham sp
                    LEFT JOIN NhomSanPham nsp ON sp.IdNhomSanPham = nsp.ID
                    LEFT JOIN ChiNhanh cn ON sp.IDChiNhanh = cn.ID
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
                            IdNhomSanPham = reader.IsDBNull(4) ? Guid.Empty : reader.GetGuid(4),
                            IDChiNhanh = reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5),
                            TenNhom = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            TenChiNhanh = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            IsDelete = reader.IsDBNull(8) ? false : reader.GetBoolean(8),
                            CreateTime = reader.IsDBNull(9) ? DateTime.Now : reader.GetDateTime(9),
                            UpdateTime = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
                        });
                    }
                }
                // Lấy danh sách nhóm sản phẩm
                var cmdNhom = new SqlCommand("SELECT ID, TenNhom FROM NhomSanPham WHERE IsDelete=0", connection);
                using (var reader = cmdNhom.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NhomSanPhams.Add(new NhomSanPhamItem
                        {
                            ID = reader.GetGuid(0),
                            TenNhom = reader.IsDBNull(1) ? "" : reader.GetString(1)
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
        // ...
    }
}
