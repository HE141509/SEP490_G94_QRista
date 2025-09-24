using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace QRB.Pages.UuDai
{
    [IgnoreAntiforgeryToken]
    public class AddUuDaiModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public AddUuDaiModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class AddUuDaiRequest
        {
            public Guid IDKhachHang { get; set; }
            public string MaGiamGia { get; set; } = string.Empty;
            public string TienGiam { get; set; } = string.Empty;
        }

        public async Task<JsonResult> OnPostAsync([FromBody] AddUuDaiRequest req)
        {
            if (req == null || req.IDKhachHang == Guid.Empty || string.IsNullOrWhiteSpace(req.MaGiamGia) || string.IsNullOrWhiteSpace(req.TienGiam))
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });
            var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"INSERT INTO Voucher (ID, IDCustomer, VoucherCode, Discount, Status, IsDelete, CreateTime) VALUES (@ID, @IDCustomer, @VoucherCode, @Discount, 0, 0, @CreateTime)", conn);
                var id = Guid.NewGuid();
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@IDCustomer", req.IDKhachHang);
                cmd.Parameters.AddWithValue("@VoucherCode", req.MaGiamGia);
                cmd.Parameters.AddWithValue("@Discount", req.TienGiam);
                cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now);
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return new JsonResult(new { success = true, id });
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = ex.Message });
                }
            }
        }
    }
}
