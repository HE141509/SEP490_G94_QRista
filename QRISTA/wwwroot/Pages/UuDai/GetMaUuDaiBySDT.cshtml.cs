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
                    SELECT mud.ID, mud.MaGiamGia, mud.TienGiam
                    FROM MaUuDai mud
                    INNER JOIN KhachHang kh ON mud.IDKhachHang = kh.ID
                    WHERE kh.SDT = @SoDienThoai AND mud.IsDelete = 0 AND mud.TrangThaiSuDung = 0
                ";
                cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new MaUuDaiDto
                        {
                            ID = reader["ID"].ToString(),
                            MaGiamGia = reader["MaGiamGia"].ToString(),
                            TienGiam = reader["TienGiam"] != DBNull.Value ? Convert.ToDecimal(reader["TienGiam"]) : 0
                        });
                    }
                }
            }
            return new JsonResult(result);
        }
    }
}
