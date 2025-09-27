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
                var noiDung = Request.Form["noiDung"];
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
                    var command = new SqlCommand(@"INSERT INTO Product (ID, ProductCode, ProductName, Picture, NoiDung, IdCategory, IDDepartment, IsDelete, CreateTime) VALUES (@ID, @MaSanPham, @TenSanPham, @HinhAnh, @NoiDung, @IdCategory, @IDChiNhanh, @IsDelete, @CreateTime)", connection);
                    var newId = Guid.NewGuid();
                    command.Parameters.AddWithValue("@ID", newId);
                    command.Parameters.AddWithValue("@MaSanPham", maSanPham.ToString());
                    command.Parameters.AddWithValue("@TenSanPham", tenSanPham.ToString());
                    command.Parameters.AddWithValue("@HinhAnh", (object?)hinhAnhData ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NoiDung", string.IsNullOrWhiteSpace(noiDung) ? DBNull.Value : noiDung.ToString());
                    command.Parameters.AddWithValue("@IdCategory", nhomSanPhamGuid);
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
            catch (SqlException ex)
            {
                string message;
                if (ex.Message.Contains("UQ_Product_ProductCode")) // constraint name
                {
                    message = "Mã sản phẩm đã tồn tại";
                }
                else
                {
                    message = "Có lỗi xảy ra: " + ex.Message;
                }

                return new JsonResult(new { success = false, message });
            }
        }

        public class AddProductRequest
        {
            public string MaSanPham { get; set; } = string.Empty;
            public string TenSanPham { get; set; } = string.Empty;
            public Guid IdCategory { get; set; }
            public Guid IDChiNhanh { get; set; }
            public bool IsDelete { get; set; }
        }
    }
}
