using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace QRB.Pages.ProductType
{
    [IgnoreAntiforgeryToken]
    public class AddProductTypeModel : PageModel
    {
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
        public bool? IsDeletePost { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var donGiaRaw = (DonGia ?? "").Replace(",", "").Replace(".", "");
                if (!long.TryParse(donGiaRaw, out var donGiaValue))
                {
                    return new JsonResult(new { success = false, message = "Đơn giá không hợp lệ!" });
                }
                if (IDSanPham == null || IDChiNhanh == null)
                {
                    return new JsonResult(new { success = false, message = "Thiếu thông tin sản phẩm hoặc chi nhánh." });
                }
                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=QRB;Trusted_Connection=True;";
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var cmd = new SqlCommand(@"INSERT INTO LoaiSanPham (ID, IDSanPham, TenLoai, MaLoai, DonGia, IsDelete, CreateTime, IDChiNhanh)
                        VALUES (@ID, @IDSanPham, @TenLoai, @MaLoai, @DonGia, @IsDelete, @CreateTime, @IDChiNhanh)", connection);
                    var newId = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("@ID", newId);
                    cmd.Parameters.AddWithValue("@IDSanPham", IDSanPham.Value);
                    cmd.Parameters.AddWithValue("@TenLoai", TenLoai ?? "");
                    cmd.Parameters.AddWithValue("@MaLoai", MaLoai ?? "");
                    cmd.Parameters.AddWithValue("@DonGia", donGiaValue);
                    cmd.Parameters.AddWithValue("@IsDelete", IsDeletePost ?? false);
                    cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@IDChiNhanh", IDChiNhanh.Value);
                    int rows = await cmd.ExecuteNonQueryAsync();
                    if (rows > 0)
                        return new JsonResult(new { success = true });
                    else
                        return new JsonResult(new { success = false, message = "Thêm loại sản phẩm thất bại!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
