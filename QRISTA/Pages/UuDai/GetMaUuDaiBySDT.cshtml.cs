using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace QRB.Pages.UuDai
{
    public class GetMaUuDaiBySDTModel : PageModel
    {
        private readonly string _connectionString;
        public GetMaUuDaiBySDTModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public class MaUuDaiDto
        {
            public string ID { get; set; }
            public string MaGiamGia { get; set; }
            public decimal TienGiam { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string soDienThoai)
        {
            if (string.IsNullOrWhiteSpace(soDienThoai))
                return new JsonResult(new List<MaUuDaiDto>());

            var result = new List<MaUuDaiDto>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT v.ID, v.VoucherCode, v.Discount
                    FROM Voucher v
                    INNER JOIN Customer c ON v.IDCustomer = c.ID
                    WHERE c.Phone = @SoDienThoai AND v.IsDelete = 0 AND v.Status = 0
                ";
                cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new MaUuDaiDto
                        {
                            ID = reader["ID"].ToString(),
                            MaGiamGia = reader["VoucherCode"].ToString(),
                            TienGiam = reader["Discount"] != DBNull.Value ? Convert.ToDecimal(reader["Discount"]) : 0
                        });
                    }
                }
            }
            return new JsonResult(result);
        }
    }
}
