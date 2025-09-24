using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System;
using System.IO;
using OrderModel = QRB.Models.Order;

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

                    var newOrder = new OrderModel
                    {
                        ID = Guid.NewGuid(),
                        OrderCode = input.MaDonHang,
                        IDCustomer = input.IDKhachHang,
                        IDEmployee = input.IDNhanVien,
                        IDDepartment = input.IDChiNhanh,
                        VoucherCode = input.MaUuDai,
                        VoucherPrice = input.TienUuDai,
                        Amount = input.TongTien,
                        Price = donGia.ToString(),
                        PaymentStatus = input.TrangThai,
                        PaymentMethod = "Tiền mặt",
                        IsDelete = false,
                        CreateTime = DateTime.Now,
                        PaymentDate = DateTime.Now,
                        Table = tableNumber
                    };

                    _context.Orders.Add(newOrder);
                    _context.SaveChanges();

                    // Lưu chi tiết đơn hàng
                    if (input.SanPhamList != null && input.SanPhamList.Count > 0)
                    {
                        foreach (var sp in input.SanPhamList)
                        {
                            var chiTiet = new OrderDetail
                            {
                                ID = Guid.NewGuid(),
                                IDOrder = newOrder.ID,
                                IDProduct = sp.IDSanPham,
                                IDProductType = sp.IDLoaiSanPham,
                                Quantity = sp.SoLuong > 0 ? sp.SoLuong : 1,
                                Price = sp.DonGia.ToString(),
                                Total = (sp.DonGia * (sp.SoLuong > 0 ? sp.SoLuong : 1)).ToString(),
                                IsDelete = false,
                                CreateTime = DateTime.Now
                            };
                            _context.OrderDetails.Add(chiTiet);
                        }
                        _context.SaveChanges();
                    }

                    // Đánh dấu mã ưu đãi đã sử dụng
                    if (!string.IsNullOrWhiteSpace(input.MaUuDai))
                    {
                        var voucher = _context.Vouchers.FirstOrDefault(v => v.VoucherCode == input.MaUuDai && !v.IsDelete);
                        if (voucher != null)
                        {
                            voucher.IsDelete = true;
                            _context.SaveChanges();
                        }
                    }

                    // Cập nhật GiaTriDonHang trong bảng Customer chỉ khi hóa đơn đã thanh toán
                    if (input.TrangThai) // Chỉ cập nhật khi hóa đơn đã được thanh toán
                    {
                        var customer = _context.Customers.FirstOrDefault(c => c.ID == input.IDKhachHang);
                        if (customer != null)
                        {
                            if (decimal.TryParse(customer.GiaTriDonHang, out decimal currentGiaTri))
                            {
                                customer.GiaTriDonHang = (currentGiaTri + tongTien).ToString();
                            }
                            else
                            {
                                customer.GiaTriDonHang = tongTien.ToString(); // Nếu không parse được, gán trực tiếp bằng TongTien
                            }
                            _context.SaveChanges();
                        }
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
