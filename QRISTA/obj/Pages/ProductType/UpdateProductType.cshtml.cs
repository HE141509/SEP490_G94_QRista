using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace QRB.Pages.ProductType
{
    [IgnoreAntiforgeryToken]
    public class UpdateProductTypeModel : PageModel
    {
        [BindProperty]
        public Guid ID { get; set; }
        [BindProperty]
        public string? TenLoai { get; set; }
        [BindProperty]
        public string? MaLoai { get; set; }
        [BindProperty]
        public string? DonGia { get; set; }
        [BindProperty]
        public Guid? IDSanPham { get; set; }
        [BindProperty]
        public Guid? IDChiNhanh { get; set; }
        [BindProperty]
        public bool? IsDelete { get; set; }

        public List<SanPhamItem> SanPhams { get; set; } = new();
        public List<ChiNhanhItem> ChiNhanhs { get; set; } = new();
        public ProductTypeListModel.LoaiSanPhamViewModel ProductType { get; set; } = new();

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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(@"SELECT lsp.ID, lsp.IDSanPham, lsp.TenLoai, lsp.MaLoai, lsp.DonGia, lsp.IsDelete, lsp.IDChiNhanh, sp.TenSanPham, cn.TenChiNhanh FROM LoaiSanPham lsp LEFT JOIN SanPham sp ON lsp.IDSanPham = sp.ID LEFT JOIN ChiNhanh cn ON lsp.IDChiNhanh = cn.ID WHERE lsp.ID = @ID", connection);
                cmd.Parameters.AddWithValue("@ID", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var result = new {
                            ID = reader.GetGuid(0),
                            IDSanPham = reader.GetGuid(1),
                            TenLoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            MaLoai = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            DonGia = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            IsDelete = reader.IsDBNull(5) ? false : reader.GetBoolean(5),
                            IDChiNhanh = reader.GetGuid(6),
                            TenSanPham = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            TenChiNhanh = reader.IsDBNull(8) ? "" : reader.GetString(8)
                        };
                        return new JsonResult(result);
                    }
                }
            }
            return new JsonResult(new { success = false, message = "Không tìm thấy loại sản phẩm!" });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Chỉ cập nhật các trường được phép sửa: TenLoai, DonGia, IsDelete
                var donGiaRaw = (DonGia ?? "").Replace(",", "").Replace(".", "");
                if (!long.TryParse(donGiaRaw, out var donGiaValue))
                {
                    return new JsonResult(new { success = false, message = "Đơn giá không hợp lệ!" });
                }
                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var cmd = new SqlCommand(@"UPDATE LoaiSanPham SET TenLoai=@TenLoai, DonGia=@DonGia, IsDelete=@IsDelete, UpdateTime=GETDATE() WHERE ID=@ID", connection);
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.Parameters.AddWithValue("@TenLoai", TenLoai ?? "");
                    cmd.Parameters.AddWithValue("@DonGia", donGiaValue);
                    cmd.Parameters.AddWithValue("@IsDelete", IsDelete ?? false);
                    await cmd.ExecuteNonQueryAsync();
                }
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}
