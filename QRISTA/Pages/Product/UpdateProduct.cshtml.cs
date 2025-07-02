using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QRB.Pages.Product
{
    public class UpdateProductModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UpdateProductModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var maSanPham = Request.Form["maSanPham"];
                var tenSanPham = Request.Form["tenSanPham"];
                var nhomSanPhamID = Request.Form["nhomSanPhamID"];
                var chiNhanhID = Request.Form["chiNhanhID"];
                var idSanPham = Request.Form["idSanPham"];
                var isDelete = Request.Form["isDelete"];
                var hinhAnhFile = Request.Form.Files["hinhAnh"];

                if (string.IsNullOrEmpty(idSanPham))
                {
                    return BadRequest("ID sản phẩm không hợp lệ.");
                }

                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query;
                    SqlCommand command;

                    // Kiểm tra xem có file ảnh mới không
                    if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                    {
                        // Cập nhật với ảnh mới
                        query = @"UPDATE SanPham 
                                SET MaSanPham = @MaSanPham,
                                    TenSanPham = @TenSanPham, 
                                    IdNhomSanPham = @IdNhomSanPham, 
                                    IDChiNhanh = @IDChiNhanh,
                                    IsDelete = @IsDelete,
                                    HinhAnh = @HinhAnh,
                                    UpdateTime = @UpdateTime
                                WHERE ID = @ID";

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@MaSanPham", maSanPham.ToString());
                        command.Parameters.AddWithValue("@TenSanPham", tenSanPham.ToString());
                        command.Parameters.AddWithValue("@IdNhomSanPham", Guid.Parse(nhomSanPhamID.ToString()));
                        command.Parameters.AddWithValue("@IDChiNhanh", Guid.Parse(chiNhanhID.ToString()));
                        command.Parameters.AddWithValue("@IsDelete", isDelete == "true");
                        command.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@ID", Guid.Parse(idSanPham.ToString()));

                        // Chuyển đổi file ảnh thành byte array
                        using (var memoryStream = new MemoryStream())
                        {
                            await hinhAnhFile.CopyToAsync(memoryStream);
                            command.Parameters.AddWithValue("@HinhAnh", memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        // Cập nhật không có ảnh mới
                        query = @"UPDATE SanPham 
                                SET MaSanPham = @MaSanPham,
                                    TenSanPham = @TenSanPham, 
                                    IdNhomSanPham = @IdNhomSanPham, 
                                    IDChiNhanh = @IDChiNhanh,
                                    IsDelete = @IsDelete,
                                    UpdateTime = @UpdateTime
                                WHERE ID = @ID";

                        command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@MaSanPham", maSanPham.ToString());
                        command.Parameters.AddWithValue("@TenSanPham", tenSanPham.ToString());
                        command.Parameters.AddWithValue("@IdNhomSanPham", Guid.Parse(nhomSanPhamID.ToString()));
                        command.Parameters.AddWithValue("@IDChiNhanh", Guid.Parse(chiNhanhID.ToString()));
                        command.Parameters.AddWithValue("@IsDelete", isDelete == "true");
                        command.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@ID", Guid.Parse(idSanPham.ToString()));
                    }

                    var result = await command.ExecuteNonQueryAsync();
                    
                    if (result > 0)
                    {
                        return new JsonResult(new { success = true, message = "Cập nhật sản phẩm thành công!" });
                    }
                    else
                    {
                        return new JsonResult(new { success = false, message = "Không thể cập nhật sản phẩm." });
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}
