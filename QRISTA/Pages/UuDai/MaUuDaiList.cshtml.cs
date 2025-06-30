using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace QRB.Pages.UuDai
{
using Microsoft.AspNetCore.Http;
public class MaUuDaiListModel : PageModel
{
    public List<MaUuDaiViewModel> MaUuDaiList { get; set; } = new();
    private readonly IConfiguration _configuration;
    public MaUuDaiListModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public IActionResult OnGet()
    {
        // Kiểm tra session login
        if (HttpContext.Session.GetString("UserId") == null)
        {
            return Redirect("/Login");
        }
        string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string sql = @"SELECT mud.ID, kh.TenKhachHang, mud.MaGiamGia, mud.TienGiam, mud.TrangThaiSuDung, mud.CreateTime, mud.UpdateTime, mud.IsDelete
                                FROM MaUuDai mud
                                JOIN KhachHang kh ON mud.IDKhachHang = kh.ID
                                ORDER BY mud.CreateTime DESC";
            using (var command = new SqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    MaUuDaiList.Add(new MaUuDaiViewModel
                    {
                        ID = reader.GetGuid(0),
                        TenKhachHang = reader.GetString(1),
                        MaGiamGia = reader.GetString(2),
                        TienGiam = reader.GetString(3),
                        TrangThaiSuDung = reader.GetBoolean(4),
                        CreateTime = reader.GetDateTime(5),
                        UpdateTime = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                        IsDelete = reader.GetBoolean(7)
                    });
                }
            }
        }
        return Page();
    }
    public class MaUuDaiViewModel
    {
        public Guid ID { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string MaGiamGia { get; set; } = string.Empty;
        public string TienGiam { get; set; } = string.Empty;
        public bool TrangThaiSuDung { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDelete { get; set; }
    }
}
}
