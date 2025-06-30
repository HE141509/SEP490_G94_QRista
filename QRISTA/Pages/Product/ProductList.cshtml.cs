using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace QRB.Pages.Product
{
    public class ProductListModel : PageModel
    {
        public class SanPhamViewModel
        {
            public Guid ID { get; set; }
            public string? MaSanPham { get; set; }
            public string? TenSanPham { get; set; }
            public Guid IdNhomSanPham { get; set; }
            public string? TenNhom { get; set; }
            public Guid IDChiNhanh { get; set; }
            public bool IsDelete { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime? UpdateTime { get; set; }
            public string? TenChiNhanh { get; set; }
        }

        public List<SanPhamViewModel> SanPhams { get; set; } = new();

        public void OnGet()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT sp.ID, sp.MaSanPham, sp.TenSanPham, 
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
                            TenNhom = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            TenChiNhanh = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            IsDelete = reader.IsDBNull(5) ? false : reader.GetBoolean(5),
                            CreateTime = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6),
                            UpdateTime = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                        });
                    }
                }
            }
        }
        // ...
    }
}
