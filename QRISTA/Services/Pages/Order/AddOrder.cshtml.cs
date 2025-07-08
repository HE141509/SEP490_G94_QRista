using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System;
using System.IO;

namespace QRB.Pages.Order
{
    public class AddOrderModel : PageModel
    {
        private readonly QRBDbContext _context;

        public AddOrderModel(QRBDbContext context)
        {
            _context = context;
        }

        public class OrderInput
        {
            public string MaDonHang { get; set; } = string.Empty;
            public Guid IDKhachHang { get; set; }
            public Guid IDNhanVien { get; set; }
            public Guid IDChiNhanh { get; set; }
            public string MaUuDai { get; set; } = string.Empty;
            public string TienUuDai { get; set; } = "0";
            public string TongTien { get; set; }
            public bool TrangThai { get; set; }
            public List<SanPhamInput> SanPhamList { get; set; } = new List<SanPhamInput>();
        }

        public class SanPhamInput
        {
            public Guid IDSanPham { get; set; }
            public Guid IDLoaiSanPham { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGia { get; set; }
        }

        public IActionResult OnPost()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = reader.ReadToEndAsync().Result;
                    var input = System.Text.Json.JsonSerializer.Deserialize<OrderInput>(body);
                    if (input == null || string.IsNullOrWhiteSpace(input.MaDonHang) || input.IDKhachHang == Guid.Empty || input.IDNhanVien == Guid.Empty || input.IDChiNhanh == Guid.Empty || decimal.Parse(input.TongTien.Replace(",","" ).Replace(".","")) <= 0)
                        return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });

                    // Tính DonGia = TongTien + TienUuDai
                    decimal tongTien = 0;
                    decimal tienUuDai = 0;
                    decimal.TryParse((input.TongTien ?? "0").Replace(",",""), out tongTien);
                    decimal.TryParse((input.TienUuDai ?? "0").Replace(",",""), out tienUuDai);
                    decimal donGia = tongTien + tienUuDai;

                    // Lấy số bàn từ session (nếu có)
                    int? tableNumber = null;
                    if (HttpContext.Session != null && HttpContext.Session.GetInt32("TableNumber") != null)
                        tableNumber = HttpContext.Session.GetInt32("TableNumber");

                    var newOrder = new DonHang
                    {
                        ID = Guid.NewGuid(),
                        MaDonHang = input.MaDonHang,
                        IDKhachHang = input.IDKhachHang,
                        IDNhanVien = input.IDNhanVien,
                        IDChiNhanh = input.IDChiNhanh,
                        MaUuDai = input.MaUuDai,
                        TienUuDai = input.TienUuDai,
                        TongTien = input.TongTien,
                        DonGia = donGia.ToString(),
                        TrangThaiThanhToan = input.TrangThai,
                        IsDelete = false,
                        CreateTime = DateTime.Now,
                        NgayThanhToan = DateTime.Now,
                        SoBan = tableNumber
                    };

                    _context.DonHangs.Add(newOrder);
                    _context.SaveChanges();

                    // Lưu chi tiết đơn hàng
                    if (input.SanPhamList != null && input.SanPhamList.Count > 0)
                    {
                        foreach (var sp in input.SanPhamList)
                        {
                            var chiTiet = new ChiTietDonHang
                            {
                                ID = Guid.NewGuid(),
                                IDDonHang = newOrder.ID,
                                IDSanPham = sp.IDSanPham,
                                IDLoaiSanPham = sp.IDLoaiSanPham,
                                SoLuong = sp.SoLuong > 0 ? sp.SoLuong : 1,
                                DonGia = sp.DonGia.ToString(),
                                ThanhTien = (sp.DonGia * (sp.SoLuong > 0 ? sp.SoLuong : 1)).ToString(),
                                IsDelete = false,
                                CreateTime = DateTime.Now
                            };
                            _context.ChiTietDonHangs.Add(chiTiet);
                        }
                        _context.SaveChanges();
                    }

                    // Đánh dấu mã ưu đãi đã sử dụng
                    if (!string.IsNullOrWhiteSpace(input.MaUuDai))
                    {
                        var maUuDai = _context.MaUuDais.FirstOrDefault(m => m.MaGiamGia == input.MaUuDai && !m.IsDelete);
                        if (maUuDai != null)
                        {
                            maUuDai.IsDelete = true;
                            _context.SaveChanges();
                        }
                    }

                    // Cập nhật GiaTriDonHang trong bảng KhachHang
                    var khachHang = _context.KhachHangs.FirstOrDefault(k => k.ID == input.IDKhachHang);
                    if (khachHang != null)
                    {
                        if (decimal.TryParse(khachHang.GiaTriDonHang, out decimal currentGiaTri))
                        {
                            khachHang.GiaTriDonHang = (currentGiaTri + tongTien).ToString();
                        }
                        else
                        {
                            khachHang.GiaTriDonHang = tongTien.ToString(); // Nếu không parse được, gán trực tiếp bằng TongTien
                        }
                        _context.SaveChanges();
                    }

                    return new JsonResult(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
