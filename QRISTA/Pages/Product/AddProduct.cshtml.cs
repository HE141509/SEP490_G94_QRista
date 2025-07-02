using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QRB.Pages.Product
{
    [IgnoreAntiforgeryToken]
    public class AddProductModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public AddProductModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var maSanPham = Request.Form["maSanPham"];
                var tenSanPham = Request.Form["tenSanPham"];
                var nhomSanPhamID = Request.Form["nhomSanPhamID"];
                var chiNhanhID = Request.Form["chiNhanhID"];
                var isDelete = Request.Form["isDelete"];
                var hinhAnhFile = Request.Form.Files["hinhAnh"];

                if (string.IsNullOrWhiteSpace(maSanPham) || string.IsNullOrWhiteSpace(tenSanPham) || 
                    !Guid.TryParse(nhomSanPhamID, out var nhomSanPhamGuid) || 
                    !Guid.TryParse(chiNhanhID, out var chiNhanhGuid))
                    return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ." });

                byte[]? hinhAnhData = null;
                if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await hinhAnhFile.CopyToAsync(memoryStream);
                        hinhAnhData = memoryStream.ToArray();
                    }
                }

                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(@"INSERT INTO SanPham (ID, MaSanPham, TenSanPham, HinhAnh, IdNhomSanPham, IDChiNhanh, IsDelete, CreateTime) VALUES (@ID, @MaSanPham, @TenSanPham, @HinhAnh, @IdNhomSanPham, @IDChiNhanh, @IsDelete, @CreateTime)", connection);
                    var newId = Guid.NewGuid();
                    command.Parameters.AddWithValue("@ID", newId);
                    command.Parameters.AddWithValue("@MaSanPham", maSanPham.ToString());
                    command.Parameters.AddWithValue("@TenSanPham", tenSanPham.ToString());
                    command.Parameters.AddWithValue("@HinhAnh", (object?)hinhAnhData ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdNhomSanPham", nhomSanPhamGuid);
                    command.Parameters.AddWithValue("@IDChiNhanh", chiNhanhGuid);
                    command.Parameters.AddWithValue("@IsDelete", isDelete == "true");
                    command.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                    int rows = command.ExecuteNonQuery();
                    if (rows > 0)
                        return new JsonResult(new { success = true });
                    else
                        return new JsonResult(new { success = false, message = "Thêm sản phẩm thất bại!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public class AddProductRequest
        {
            public string MaSanPham { get; set; } = string.Empty;
            public string TenSanPham { get; set; } = string.Empty;
            public Guid IdNhomSanPham { get; set; }
            public Guid IDChiNhanh { get; set; }
            public bool IsDelete { get; set; }
        }
    }
}
