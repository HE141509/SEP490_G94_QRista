using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;
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

        public string UserName { get; private set; } = string.Empty;
        public string CurrentUserRole { get; private set; } = string.Empty;
        public bool IsLoggedIn { get; private set; }
        public List<QRB.Models.ChiNhanh> Branches { get; set; } = new List<QRB.Models.ChiNhanh>();

        // Bộ lọc ngày
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BranchId { get; set; }

        // Thống kê bán hàng
        public int SoLuotNguoiMuaTrongNgay { get; set; }
        public decimal TongGiaTriMua { get; set; }
        public int SoLuongSanPhamMua { get; set; }
        public int SoLuongHoaDonTrongNgay { get; set; }
        public int SoKhachMoiDangKy { get; set; }

        // So sánh với khoảng thời gian trước đó
        public int ThayDoiNguoiMua { get; set; }
        public decimal ThayDoiGiaTri { get; set; }
        public int ThayDoiSanPham { get; set; }
        public int ThayDoiHoaDon { get; set; }
        public int ThayDoiKhachMoi { get; set; }

        // Dữ liệu cho biểu đồ biến động
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<int> ChartOrderData { get; set; } = new List<int>();
        public List<int> ChartQuantityData { get; set; } = new List<int>();
        public List<decimal> ChartRevenueData { get; set; } = new List<decimal>();

        // Dữ liệu cho biểu đồ phân tích thứ trong tuần
        public List<string> WeekdayLabels { get; set; } = new List<string>();
        public List<int> WeekdayOrderData { get; set; } = new List<int>();

        // Dữ liệu cho biểu đồ phân tích số bàn
        public List<string> TableLabels { get; set; } = new List<string>();
        public List<int> TableUsageData { get; set; } = new List<int>();

        // Dữ liệu cho danh sách sản phẩm bán chạy
        public List<TopProductModel> TopProducts { get; set; } = new List<TopProductModel>();

        // Lưu danh sách chi nhánh của user để tránh query lặp
        private List<Guid> _userBranchIds = new List<Guid>();

        private async Task<List<Guid>> GetUserBranchIds()
        {
            // Clear cache nếu có parameter BranchId từ dropdown để đảm bảo load dữ liệu mới
            if (!string.IsNullOrEmpty(BranchId) && _userBranchIds.Any())
            {
                Console.WriteLine($"Dashboard - Clearing cache because BranchId parameter exists: {BranchId}");
                _userBranchIds.Clear();
            }
            
            if (_userBranchIds.Any()) return _userBranchIds;

            // Nếu user chọn chi nhánh cụ thể từ dropdown
            if (!string.IsNullOrEmpty(BranchId) && BranchId != "tatca")
            {
                if (Guid.TryParse(BranchId, out Guid selectedBranchGuid))
                {
                    _userBranchIds.Add(selectedBranchGuid);
                    Console.WriteLine($"Dashboard - Using selected branch: {selectedBranchGuid}");
                    return _userBranchIds;
                }
            }

            // Nếu chọn "Tất cả" hoặc không chọn gì, lấy TẤT CẢ chi nhánh của user từ UserBranches
            var userId = HttpContext.Session.GetString("UserId");
            Console.WriteLine($"Dashboard - Getting all branches for UserId: {userId ?? "NULL"}");
            
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                _userBranchIds = await _context.UserBranches
                    .Where(ub => ub.UserId == userGuid && ub.IsActive)
                    .Select(ub => ub.BranchId)
                    .ToListAsync();
                Console.WriteLine($"Dashboard - Found {_userBranchIds.Count} branches from UserBranches for 'Tất cả'");
                
                if (_userBranchIds.Any())
                {
                    return _userBranchIds;
                }
            }
            
            // Fallback: Nếu không tìm thấy trong UserBranches, dùng chi nhánh từ session
            var chiNhanhId = HttpContext.Session.GetString("ChiNhanhId");
            Console.WriteLine($"Dashboard - Fallback - ChiNhanhId from session: {chiNhanhId ?? "NULL"}");
            
            if (Guid.TryParse(chiNhanhId, out Guid branchGuid))
            {
                _userBranchIds.Add(branchGuid);
                Console.WriteLine($"Dashboard - Using branch from session as fallback: {branchGuid}");
                return _userBranchIds;
            }
            
            return _userBranchIds;
        }

        private bool HasPermission(string permissionName)
        {
            var permissionsJson = HttpContext.Session.GetString("UserPermissions");
            if (string.IsNullOrEmpty(permissionsJson))
            {
                return false;
            }
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
                // Chưa đăng nhập, chuyển về trang login
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

            // Lấy thống kê bán hàng
            await LoadSalesStatistics();

            // Lấy danh sách chi nhánh cho combobox
            var userBranchIds = await GetUserBranchIds();
            var departments = await _context.Departments
                .Where(cn => !cn.IsDelete && userBranchIds.Contains(cn.ID))
                .OrderBy(cn => cn.DepartmentName)
                .ToListAsync();
            
            Branches = departments.Select(d => new ChiNhanh(d)).ToList();
            // Lấy dữ liệu biểu đồ
            await LoadChartData();

            // Lấy dữ liệu biểu đồ phân tích
            await LoadAnalysisChartData();

            // Lấy dữ liệu sản phẩm bán chạy
            await LoadTopProducts();

            return Page();
        }

        private async Task LoadSalesStatistics()
        {
            var fromDate = FromDate ?? DateTime.Today;
            var toDate = ToDate ?? DateTime.Today;

            // Đảm bảo toDate bao gồm cả ngày (đến 23:59:59)
            var toDateEnd = toDate.Date.AddDays(1).AddSeconds(-1);

            // Tính khoảng thời gian trước đó để so sánh
            var daysDiff = (toDate - fromDate).Days + 1;
            var compareFromDate = fromDate.AddDays(-daysDiff);
            var compareToDate = fromDate.AddSeconds(-1);

            // Sử dụng hàm GetUserBranchIds() để lấy chi nhánh (đã bao gồm logic dropdown)
            var userBranchIds = await GetUserBranchIds();
            Console.WriteLine($"LoadSalesStatistics - Using branch IDs: {string.Join(", ", userBranchIds)}");

            try
            {
                // Thống kê trong khoảng thời gian được chọn - có filter theo chi nhánh
                var ordersInPeriod = await _context.Orders
                    .Where(d => d.CreateTime >= fromDate && d.CreateTime <= toDateEnd && d.PaymentStatus == true 
                               && userBranchIds.Contains(d.IDDepartment))
                    .Include(d => d.OrderDetails)
                    .ToListAsync();

                // Thống kê trong khoảng thời gian trước đó (để so sánh) - có filter theo chi nhánh
                var ordersInComparePeriod = await _context.Orders
                    .Where(d => d.CreateTime >= compareFromDate && d.CreateTime <= compareToDate && d.PaymentStatus == true
                               && userBranchIds.Contains(d.IDDepartment))
                    .Include(d => d.OrderDetails)
                    .ToListAsync();

                // Số lượt người mua trong khoảng thời gian (số khách hàng unique)
                SoLuotNguoiMuaTrongNgay = ordersInPeriod.Where(d => d.IDCustomer.HasValue).Select(d => d.IDCustomer).Distinct().Count();
                var nguoiMuaTruocDo = ordersInComparePeriod.Where(d => d.IDCustomer.HasValue).Select(d => d.IDCustomer).Distinct().Count();
                ThayDoiNguoiMua = SoLuotNguoiMuaTrongNgay - nguoiMuaTruocDo;

                // Tổng giá trị mua (chuyển đổi từ string sang decimal)
                TongGiaTriMua = ordersInPeriod.Sum(d => decimal.TryParse(d.Amount, out var amount) ? amount : 0);
                var giaTriTruocDo = ordersInComparePeriod.Sum(d => decimal.TryParse(d.Amount, out var amount) ? amount : 0);
                ThayDoiGiaTri = TongGiaTriMua - giaTriTruocDo;

                // Số lượng sản phẩm mua
                SoLuongSanPhamMua = ordersInPeriod.SelectMany(d => d.OrderDetails).Sum(ct => ct.Quantity);
                var sanPhamTruocDo = ordersInComparePeriod.SelectMany(d => d.OrderDetails).Sum(ct => ct.Quantity);
                ThayDoiSanPham = SoLuongSanPhamMua - sanPhamTruocDo;

                // Số lượng hóa đơn trong khoảng thời gian
                SoLuongHoaDonTrongNgay = ordersInPeriod.Count;
                var hoaDonTruocDo = ordersInComparePeriod.Count;
                ThayDoiHoaDon = SoLuongHoaDonTrongNgay - hoaDonTruocDo;

                // Số khách mới đăng ký trong khoảng thời gian
                SoKhachMoiDangKy = await _context.Customers
                    .Where(c => c.CreateTime >= fromDate && c.CreateTime <= toDateEnd && !c.IsDelete)
                    .CountAsync();
                var khachMoiTruocDo = await _context.Customers
                    .Where(c => c.CreateTime >= compareFromDate && c.CreateTime <= compareToDate && !c.IsDelete)
                    .CountAsync();
                ThayDoiKhachMoi = SoKhachMoiDangKy - khachMoiTruocDo;
            }
            catch
            {
                // Set giá trị mặc định khi có lỗi
                SoLuotNguoiMuaTrongNgay = 0;
                TongGiaTriMua = 0;
                SoLuongSanPhamMua = 0;
                SoLuongHoaDonTrongNgay = 0;
                SoKhachMoiDangKy = 0;
                ThayDoiNguoiMua = 0;
                ThayDoiGiaTri = 0;
                ThayDoiSanPham = 0;
                ThayDoiHoaDon = 0;
                ThayDoiKhachMoi = 0;
            }
        }

        private async Task LoadChartData()
        {
            var fromDate = FromDate ?? DateTime.Today;
            var toDate = ToDate ?? DateTime.Today;

            // Sử dụng hàm GetUserBranchIds() để lấy chi nhánh (đã bao gồm logic dropdown)
            var userBranchIds = await GetUserBranchIds();
            Console.WriteLine($"LoadChartData - Using branch IDs: {string.Join(", ", userBranchIds)}");

            // Tạo danh sách các ngày trong khoảng thời gian
            var dates = new List<DateTime>();
            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            ChartLabels.Clear();
            ChartOrderData.Clear();
            ChartQuantityData.Clear();
            ChartRevenueData.Clear();

            foreach (var date in dates)
            {
                // Lấy dữ liệu cho ngày này - có filter theo chi nhánh
                var dayStart = date.Date;
                var dayEnd = date.Date.AddDays(1).AddSeconds(-1);

                var ordersOnDay = await _context.Orders
                    .Where(d => d.CreateTime >= dayStart && d.CreateTime <= dayEnd && d.PaymentStatus == true
                               && userBranchIds.Contains(d.IDDepartment))
                    .Include(d => d.OrderDetails)
                    .ToListAsync();

                // Thêm vào biểu đồ
                ChartLabels.Add(date.ToString("dd/MM"));
                ChartOrderData.Add(ordersOnDay.Count);
                ChartQuantityData.Add(ordersOnDay.SelectMany(d => d.OrderDetails).Sum(ct => ct.Quantity));
                
                // Tính doanh số của ngày này
                var revenueOnDay = ordersOnDay.SelectMany(d => d.OrderDetails).Sum(ct => 
                    ct.Quantity * (decimal.TryParse(ct.Price, out var price) ? price : 0));
                ChartRevenueData.Add(revenueOnDay);
            }
        }

        private async Task LoadAnalysisChartData()
        {
            var fromDate = FromDate ?? DateTime.Today;
            var toDate = ToDate ?? DateTime.Today;
            var toDateEnd = toDate.Date.AddDays(1).AddSeconds(-1);

            // Lấy danh sách chi nhánh của user hiện tại
            var userBranchIds = await GetUserBranchIds();

            try
            {
                // 1. Phân tích theo thứ trong tuần - có filter theo chi nhánh
                var weekdayNames = new[] { "Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy" };
                var weekdayData = new int[7];

                var allOrders = await _context.Orders
                    .Where(d => d.CreateTime >= fromDate && d.CreateTime <= toDateEnd && d.PaymentStatus == true
                               && userBranchIds.Contains(d.IDDepartment))
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

                // 2. Phân tích theo số bàn - có filter theo chi nhánh
                var tableUsage = await _context.Orders
                    .Where(d => d.CreateTime >= fromDate && d.CreateTime <= toDateEnd && d.PaymentStatus == true && d.Table.HasValue
                               && userBranchIds.Contains(d.IDDepartment))
                    .GroupBy(d => d.Table!.Value)
                    .Select(g => new { TableNumber = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10) // Lấy top 10 bàn được sử dụng nhiều nhất
                    .ToListAsync();

                TableLabels.Clear();
                TableUsageData.Clear();
                foreach (var table in tableUsage)
                {
                    TableLabels.Add($"Bàn {table.TableNumber}");
                    TableUsageData.Add(table.Count);
                }

                // Nếu không có dữ liệu bàn, tạo dữ liệu mẫu
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
                // Dữ liệu mặc định khi có lỗi
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

                // Lấy danh sách chi nhánh của user hiện tại
                var userBranchIds = await GetUserBranchIds();

                // Truy vấn tất cả chi tiết đơn hàng trong khoảng thời gian - có filter theo chi nhánh
                var allOrderDetails = await _context.OrderDetails
                    .Where(ct => ct.Order.CreateTime >= fromDate && ct.Order.CreateTime <= toDateEnd && !ct.IsDelete
                                && userBranchIds.Contains(ct.Order.IDDepartment))
                    .Include(ct => ct.Product)
                    .Include(ct => ct.Order)
                    .ToListAsync();

                // Nếu không có dữ liệu trong khoảng thời gian, thử mở rộng ra 30 ngày gần nhất
                if (!allOrderDetails.Any())
                {
                    var extendedFromDate = DateTime.Today.AddDays(-30);
                    allOrderDetails = await _context.OrderDetails
                        .Where(ct => ct.Order.CreateTime >= extendedFromDate && !ct.IsDelete
                                    && userBranchIds.Contains(ct.Order.IDDepartment))
                        .Include(ct => ct.Product)
                        .Include(ct => ct.Order)
                        .ToListAsync();
                }

                TopProducts.Clear();

                if (allOrderDetails.Any())
                {
                    // Nhóm theo sản phẩm và tính tổng
                    var groupedProducts = allOrderDetails
                        .GroupBy(ct => new { ct.IDProduct, ct.Product.TenSanPham, ct.Product.MaSanPham })
                        .Select(g => new
                        {
                            IDSanPham = g.Key.IDProduct,
                            TenSanPham = g.Key.TenSanPham,
                            MaSanPham = g.Key.MaSanPham,
                            TongSoLuong = g.Sum(ct => ct.Quantity),
                            TongDoanhThu = g.Sum(ct => decimal.TryParse(ct.Total, out decimal value) ? value : 0)
                        })
                        .OrderByDescending(x => x.TongSoLuong)
                        .Take(5)
                        .ToList();

                    // Xử lý xếp hạng với icon và màu sắc
                    for (int i = 0; i < groupedProducts.Count; i++)
                    {
                        var product = groupedProducts[i];
                        var rank = i + 1;

                        string rankIcon = "";
                        string rankColor = "";

                        switch (rank)
                        {
                            case 1:
                                rankIcon = "fas fa-trophy";
                                rankColor = "#FFD700"; // Vàng
                                break;
                            case 2:
                                rankIcon = "fas fa-medal";
                                rankColor = "#C0C0C0"; // Bạc
                                break;
                            case 3:
                                rankIcon = "fas fa-award";
                                rankColor = "#CD7F32"; // Đồng
                                break;
                            case 4:
                                rankIcon = "fas fa-star";
                                rankColor = "#4A90E2"; // Xanh dương
                                break;
                            case 5:
                                rankIcon = "fas fa-crown";
                                rankColor = "#9B59B6"; // Tím
                                break;
                        }

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
                    // Tạo dữ liệu mẫu khi không có dữ liệu thực
                    CreateSampleTopProducts();
                }
            }
            catch
            {
                // Tạo dữ liệu mẫu khi có lỗi
                CreateSampleTopProducts();
                // TODO: Log exception nếu cần
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

                string rankIcon = "";
                string rankColor = "";

                switch (rank)
                {
                    case 1:
                        rankIcon = "fas fa-trophy";
                        rankColor = "#FFD700";
                        break;
                    case 2:
                        rankIcon = "fas fa-medal";
                        rankColor = "#C0C0C0";
                        break;
                    case 3:
                        rankIcon = "fas fa-award";
                        rankColor = "#CD7F32";
                        break;
                    case 4:
                        rankIcon = "fas fa-star";
                        rankColor = "#4A90E2";
                        break;
                    case 5:
                        rankIcon = "fas fa-crown";
                        rankColor = "#9B59B6";
                        break;
                }

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
            // Xóa session
            HttpContext.Session.Clear();

            // Xóa cookie remember me nếu có
            if (Request.Cookies.ContainsKey("RememberMe"))
            {
                Response.Cookies.Delete("RememberMe");
            }

            return RedirectToPage("/Login");
        }
    }

    // Model cho sản phẩm bán chạy
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
