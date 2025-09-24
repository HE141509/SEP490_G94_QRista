using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using System.Text.Json;

namespace QRB.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly QRBDbContext _context;
        public DashboardModel(QRBDbContext context)
        {
            _context = context;
        }

        // Thông tin user
        public string UserName { get; private set; } = string.Empty;
        public string CurrentUserRole { get; private set; } = string.Empty;
        public bool IsLoggedIn { get; private set; }

        // Bộ lọc ngày
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        // Thống kê bán hàng
        public int SoLuotNguoiMuaTrongNgay { get; set; }
        public decimal TongGiaTriMua { get; set; }
        public int SoLuongSanPhamMua { get; set; }
        public int SoLuongHoaDonTrongNgay { get; set; }

        // So sánh với khoảng thời gian trước đó
        public int ThayDoiNguoiMua { get; set; }
        public decimal ThayDoiGiaTri { get; set; }
        public int ThayDoiSanPham { get; set; }
        public int ThayDoiHoaDon { get; set; }

        // Dữ liệu cho biểu đồ biến động
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartOrderData { get; set; } = new();
        public List<int> ChartQuantityData { get; set; } = new();

        // Dữ liệu cho biểu đồ phân tích thứ trong tuần
        public List<string> WeekdayLabels { get; set; } = new();
        public List<int> WeekdayOrderData { get; set; } = new();

        // Dữ liệu cho biểu đồ phân tích số bàn
        public List<string> TableLabels { get; set; } = new();
        public List<int> TableUsageData { get; set; } = new();

        // Dữ liệu cho danh sách sản phẩm bán chạy
        public List<TopProductModel> TopProducts { get; set; } = new();

        private bool HasPermission(string permissionName)
        {
            var permissionsJson = HttpContext.Session.GetString("UserPermissions");
            if (string.IsNullOrEmpty(permissionsJson))
                return false;

            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
                return permissions?.Contains(permissionName) ?? false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra session đăng nhập
            var userId = HttpContext.Session.GetString("UserId");
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Login");
            }

            if (!HasPermission("Full Dashboard"))
            {
                return Redirect($"/AccessDenied?permission=Full Dashboard&module=Dashboard");
            }

            IsLoggedIn = true;
            UserName = HttpContext.Session.GetString("DisplayName") ?? username;
            CurrentUserRole = HttpContext.Session.GetString("VaiTro") ?? "Người dùng";

            // Thiết lập ngày mặc định nếu chưa có
            if (!FromDate.HasValue)
                FromDate = DateTime.Today;
            if (!ToDate.HasValue)
                ToDate = DateTime.Today;

            // Load dữ liệu dashboard
            await LoadSalesStatistics();
            await LoadChartData();
            await LoadAnalysisChartData();
            await LoadTopProducts();

            return Page();
        }

        private async Task LoadSalesStatistics()
        {
            var fromDate = FromDate ?? DateTime.Today;
            var toDate = ToDate ?? DateTime.Today;
            var toDateEnd = toDate.Date.AddDays(1).AddSeconds(-1);

            // Khoảng thời gian so sánh
            var daysDiff = (toDate - fromDate).Days + 1;
            var compareFromDate = fromDate.AddDays(-daysDiff);
            var compareToDate = fromDate.AddSeconds(-1);

            try
            {
                var ordersInPeriod = await _context.DonHangs
                    .Where(d => d.CreateTime >= fromDate && d.CreateTime <= toDateEnd && d.TrangThaiThanhToan == true)
                    .Include(d => d.ChiTietDonHangs)
                    .ToListAsync();

                var ordersInComparePeriod = await _context.DonHangs
                    .Where(d => d.CreateTime >= compareFromDate && d.CreateTime <= compareToDate && d.TrangThaiThanhToan == true)
                    .Include(d => d.ChiTietDonHangs)
                    .ToListAsync();

                // Người mua
                SoLuotNguoiMuaTrongNgay = ordersInPeriod.Where(d => d.IDKhachHang.HasValue)
                                                        .Select(d => d.IDKhachHang)
                                                        .Distinct()
                                                        .Count();
                var nguoiMuaTruocDo = ordersInComparePeriod.Where(d => d.IDKhachHang.HasValue)
                                                           .Select(d => d.IDKhachHang)
                                                           .Distinct()
                                                           .Count();
                ThayDoiNguoiMua = SoLuotNguoiMuaTrongNgay - nguoiMuaTruocDo;

                // Tổng giá trị mua
                TongGiaTriMua = ordersInPeriod.Sum(d => decimal.TryParse(d.TongTien, out var tongTien) ? tongTien : 0);
                var giaTriTruocDo = ordersInComparePeriod.Sum(d => decimal.TryParse(d.TongTien, out var tongTien) ? tongTien : 0);
                ThayDoiGiaTri = TongGiaTriMua - giaTriTruocDo;

                // Sản phẩm
                SoLuongSanPhamMua = ordersInPeriod.SelectMany(d => d.ChiTietDonHangs).Sum(ct => ct.SoLuong);
                var sanPhamTruocDo = ordersInComparePeriod.SelectMany(d => d.ChiTietDonHangs).Sum(ct => ct.SoLuong);
                ThayDoiSanPham = SoLuongSanPhamMua - sanPhamTruocDo;

                // Hóa đơn
                SoLuongHoaDonTrongNgay = ordersInPeriod.Count;
                var hoaDonTruocDo = ordersInComparePeriod.Count;
                ThayDoiHoaDon = SoLuongHoaDonTrongNgay - hoaDonTruocDo;
            }
            catch
            {
                SoLuotNguoiMuaTrongNgay = 0;
                TongGiaTriMua = 0;
                SoLuongSanPhamMua = 0;
                SoLuongHoaDonTrongNgay = 0;
                ThayDoiNguoiMua = 0;
                ThayDoiGiaTri = 0;
                ThayDoiSanPham = 0;
                ThayDoiHoaDon = 0;
            }
        }

        private async Task LoadChartData()
        {
            var fromDate = FromDate ?? DateTime.Today;
            var toDate = ToDate ?? DateTime.Today;

            var dates = new List<DateTime>();
            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            ChartLabels.Clear();
            ChartOrderData.Clear();
            ChartQuantityData.Clear();

            foreach (var date in dates)
            {
                var dayStart = date.Date;
                var dayEnd = date.Date.AddDays(1).AddSeconds(-1);

                var ordersOnDay = await _context.DonHangs
                    .Where(d => d.CreateTime >= dayStart && d.CreateTime <= dayEnd && d.TrangThaiThanhToan == true)
                    .Include(d => d.ChiTietDonHangs)
                    .ToListAsync();

                ChartLabels.Add(date.ToString("dd/MM"));
                ChartOrderData.Add(ordersOnDay.Count);
                ChartQuantityData.Add(ordersOnDay.SelectMany(d => d.ChiTietDonHangs).Sum(ct => ct.SoLuong));
            }
        }

        private async Task LoadAnalysisChartData()
        {
            var fromDate = FromDate ?? DateTime.Today;
            var toDate = ToDate ?? DateTime.Today;
            var toDateEnd = toDate.Date.AddDays(1).AddSeconds(-1);

            try
            {
                // 1. Theo thứ trong tuần
                var weekdayNames = new[] { "Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy" };
                var weekdayData = new int[7];

                var allOrders = await _context.DonHangs
                    .Where(d => d.CreateTime >= fromDate && d.CreateTime <= toDateEnd && d.TrangThaiThanhToan == true)
                    .ToListAsync();

                foreach (var order in allOrders)
                {
                    var dayOfWeek = (int)order.CreateTime.DayOfWeek;
                    weekdayData[dayOfWeek]++;
                }

                WeekdayLabels.Clear();
                WeekdayOrderData.Clear();
                for (int i = 0; i < 7; i++)
                {
                    WeekdayLabels.Add(weekdayNames[i]);
                    WeekdayOrderData.Add(weekdayData[i]);
                }

                // 2. Theo số bàn
                var tableUsage = await _context.DonHangs
                    .Where(d => d.CreateTime >= fromDate && d.CreateTime <= toDateEnd && d.TrangThaiThanhToan == true && d.SoBan.HasValue)
                    .GroupBy(d => d.SoBan!.Value)
                    .Select(g => new { TableNumber = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync();

                TableLabels.Clear();
                TableUsageData.Clear();
                foreach (var table in tableUsage)
                {
                    TableLabels.Add($"Bàn {table.TableNumber}");
                    TableUsageData.Add(table.Count);
                }

                if (!TableLabels.Any())
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        TableLabels.Add($"Bàn {i}");
                        TableUsageData.Add(0);
                    }
                }
            }
            catch
            {
                WeekdayLabels.AddRange(new[] { "Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy" });
                WeekdayOrderData.AddRange(new[] { 0, 0, 0, 0, 0, 0, 0 });
                TableLabels.AddRange(new[] { "Bàn 1", "Bàn 2", "Bàn 3", "Bàn 4", "Bàn 5" });
                TableUsageData.AddRange(new[] { 0, 0, 0, 0, 0 });
            }
        }

        private async Task LoadTopProducts()
        {
            try
            {
                var fromDate = FromDate ?? DateTime.Today;
                var toDate = ToDate ?? DateTime.Today;
                var toDateEnd = toDate.Date.AddDays(1).AddSeconds(-1);

                var allChiTietDonHangs = await _context.ChiTietDonHangs
                    .Where(ct => ct.DonHang.CreateTime >= fromDate && ct.DonHang.CreateTime <= toDateEnd && !ct.IsDelete)
                    .Include(ct => ct.SanPham)
                    .Include(ct => ct.DonHang)
                    .ToListAsync();

                if (!allChiTietDonHangs.Any())
                {
                    var extendedFromDate = DateTime.Today.AddDays(-30);
                    allChiTietDonHangs = await _context.ChiTietDonHangs
                        .Where(ct => ct.DonHang.CreateTime >= extendedFromDate && !ct.IsDelete)
                        .Include(ct => ct.SanPham)
                        .Include(ct => ct.DonHang)
                        .ToListAsync();
                }

                TopProducts.Clear();
                if (allChiTietDonHangs.Any())
                {
                    var groupedProducts = allChiTietDonHangs
                        .GroupBy(ct => new { ct.IDSanPham, ct.SanPham.TenSanPham, ct.SanPham.MaSanPham })
                        .Select(g => new
                        {
                            g.Key.IDSanPham,
                            g.Key.TenSanPham,
                            g.Key.MaSanPham,
                            TongSoLuong = g.Sum(ct => ct.SoLuong),
                            TongDoanhThu = g.Sum(ct => decimal.TryParse(ct.ThanhTien, out decimal value) ? value : 0)
                        })
                        .OrderByDescending(x => x.TongSoLuong)
                        .Take(5)
                        .ToList();

                    for (int i = 0; i < groupedProducts.Count; i++)
                    {
                        var product = groupedProducts[i];
                        var rank = i + 1;

                        (string rankIcon, string rankColor) = rank switch
                        {
                            1 => ("fas fa-trophy", "#FFD700"),
                            2 => ("fas fa-medal", "#C0C0C0"),
                            3 => ("fas fa-award", "#CD7F32"),
                            4 => ("fas fa-star", "#4A90E2"),
                            5 => ("fas fa-crown", "#9B59B6"),
                            _ => ("", "")
                        };

                        TopProducts.Add(new TopProductModel
                        {
                            TenSanPham = product.TenSanPham,
                            MaSanPham = product.MaSanPham,
                            TongSoLuong = product.TongSoLuong,
                            TongDoanhThu = product.TongDoanhThu,
                            Rank = rank,
                            RankIcon = rankIcon,
                            RankColor = rankColor
                        });
                    }
                }
                else
                {
                    CreateSampleTopProducts();
                }
            }
            catch
            {
                CreateSampleTopProducts();
            }
        }

        private void CreateSampleTopProducts()
        {
            var sampleProducts = new[]
            {
                new { Name = "Cà phê đen", Code = "CF001", Qty = 0, Revenue = 0m },
                new { Name = "Cà phê sữa", Code = "CF002", Qty = 0, Revenue = 0m },
                new { Name = "Bánh mì", Code = "BM001", Qty = 0, Revenue = 0m },
                new { Name = "Nước cam", Code = "NC001", Qty = 0, Revenue = 0m },
                new { Name = "Trà đá", Code = "TD001", Qty = 0, Revenue = 0m }
            };

            TopProducts.Clear();
            for (int i = 0; i < sampleProducts.Length; i++)
            {
                var product = sampleProducts[i];
                var rank = i + 1;

                (string rankIcon, string rankColor) = rank switch
                {
                    1 => ("fas fa-trophy", "#FFD700"),
                    2 => ("fas fa-medal", "#C0C0C0"),
                    3 => ("fas fa-award", "#CD7F32"),
                    4 => ("fas fa-star", "#4A90E2"),
                    5 => ("fas fa-crown", "#9B59B6"),
                    _ => ("", "")
                };

                TopProducts.Add(new TopProductModel
                {
                    TenSanPham = product.Name,
                    MaSanPham = product.Code,
                    TongSoLuong = product.Qty,
                    TongDoanhThu = product.Revenue,
                    Rank = rank,
                    RankIcon = rankIcon,
                    RankColor = rankColor
                });
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();

            if (Request.Cookies.ContainsKey("RememberMe"))
            {
                Response.Cookies.Delete("RememberMe");
            }

            return RedirectToPage("/Login");
        }
    }

    public class TopProductModel
    {
        public string TenSanPham { get; set; } = string.Empty;
        public string MaSanPham { get; set; } = string.Empty;
        public int TongSoLuong { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int Rank { get; set; }
        public string RankIcon { get; set; } = string.Empty;
        public string RankColor { get; set; } = string.Empty;
    }
}
