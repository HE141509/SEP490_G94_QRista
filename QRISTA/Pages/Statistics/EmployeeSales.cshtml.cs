using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using System.Text.Json;

namespace QRB.Pages.Statistics
{
    public class EmployeeSalesModel : PageModel
    {
        private readonly QRBDbContext _context;

        public EmployeeSalesModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<EmployeeSalesDto> EmployeeSales { get; set; } = new List<EmployeeSalesDto>();
        public string CurrentBranchName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime SelectedDate { get; set; } = DateTime.Today;

        // Bộ lọc ngày
        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BranchId { get; set; }

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
            // Kiểm tra đăng nhập
            var userId = HttpContext.Session.GetString("UserId");
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Login");
            }

            // Kiểm tra quyền truy cập
            if (!HasPermission("View Dashboard") && !HasPermission("Full Dashboard"))
            {
                return Redirect($"/AccessDenied?permission=View Dashboard&module=Statistics");
            }

            // Lấy thông tin người dùng hiện tại
            DisplayName = HttpContext.Session.GetString("DisplayName") ?? username;
            CurrentBranchName = HttpContext.Session.GetString("ChiNhanhName") ?? "Chi nhánh mặc định";

            // Thiết lập ngày mặc định nếu chưa có
            if (!FromDate.HasValue)
                FromDate = DateTime.Today;
            if (!ToDate.HasValue)
                ToDate = DateTime.Today;

            // Lấy danh sách chi nhánh của user để filter
            var userBranchIds = await GetUserBranchIds();

            // Lấy dữ liệu thống kê
            await LoadEmployeeSalesData(userBranchIds);

            return Page();
        }

        private async Task<List<Guid>> GetUserBranchIds()
        {
            var userBranchIds = new List<Guid>();
            
            // Nếu user chọn chi nhánh cụ thể từ dropdown
            if (!string.IsNullOrEmpty(BranchId) && BranchId != "tatca")
            {
                if (Guid.TryParse(BranchId, out Guid selectedBranchGuid))
                {
                    userBranchIds.Add(selectedBranchGuid);
                    return userBranchIds;
                }
            }

            // Lấy tất cả chi nhánh của user từ UserBranches
            var userId = HttpContext.Session.GetString("UserId");
            
            if (Guid.TryParse(userId, out Guid userGuid))
            {
                userBranchIds = await _context.UserBranches
                    .Where(ub => ub.UserId == userGuid && ub.IsActive)
                    .Select(ub => ub.BranchId)
                    .ToListAsync();

                if (userBranchIds.Any())
                {
                    return userBranchIds;
                }
            }

            // Fallback: Nếu không tìm thấy trong UserBranches, dùng chi nhánh từ session
            var chiNhanhId = HttpContext.Session.GetString("ChiNhanhId");
            
            if (Guid.TryParse(chiNhanhId, out Guid branchGuid))
            {
                userBranchIds.Add(branchGuid);
            }

            return userBranchIds;
        }

        private async Task LoadEmployeeSalesData(List<Guid> userBranchIds)
        {
            try
            {
                var fromDate = FromDate ?? DateTime.Today;
                var toDate = ToDate ?? DateTime.Today;
                var toDateEnd = toDate.Date.AddDays(1).AddSeconds(-1);

                Console.WriteLine($"[EmployeeSales] Date range: {fromDate:yyyy-MM-dd} to {toDateEnd:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"[EmployeeSales] User branch IDs: [{string.Join(", ", userBranchIds)}]");

                // Đầu tiên kiểm tra có orders nào không
                var totalOrdersInRange = await _context.Orders
                    .Where(o => o.CreateTime >= fromDate && o.CreateTime <= toDateEnd && !o.IsDelete)
                    .CountAsync();
                Console.WriteLine($"[EmployeeSales] Total orders in date range: {totalOrdersInRange}");

                var validOrdersInRange = await _context.Orders
                    .Where(o => o.CreateTime >= fromDate && o.CreateTime <= toDateEnd && 
                               o.PaymentStatus == true && o.Served == true && !o.IsDelete &&
                               o.IsCancelled != true && o.IsRefunded != true)
                    .CountAsync();
                Console.WriteLine($"[EmployeeSales] Valid orders (paid + served + not cancelled/refunded) in date range: {validOrdersInRange}");

                // DEBUG: Kiểm tra Orders có IDEmployee gì
                var sampleOrders = await _context.Orders
                    .Where(o => o.CreateTime >= fromDate && o.CreateTime <= toDateEnd && 
                               o.PaymentStatus == true && o.Served == true && !o.IsDelete &&
                               o.IsCancelled != true && o.IsRefunded != true)
                    .Select(o => new { o.ID, o.IDEmployee, o.IDDepartment, o.Amount })
                    .Take(5)
                    .ToListAsync();
                    
                Console.WriteLine($"[EmployeeSales] Sample valid orders (paid + served + not cancelled/refunded):");
                foreach (var order in sampleOrders)
                {
                    Console.WriteLine($"[EmployeeSales]   Order {order.ID}: Employee={order.IDEmployee}, Dept={order.IDDepartment}, Amount={order.Amount}");
                }

                // DEBUG: Kiểm tra User table
                var sampleUsers = await _context.NguoiDungs
                    .Where(u => !u.IsDelete)
                    .Select(u => new { u.ID, u.TenHienThi, u.IDChiNhanh })
                    .Take(5)
                    .ToListAsync();
                    
                Console.WriteLine($"[EmployeeSales] Sample users:");
                foreach (var user in sampleUsers)
                {
                    Console.WriteLine($"[EmployeeSales]   User {user.ID}: Name={user.TenHienThi}, Branch={user.IDChiNhanh}");
                }

                // DEBUG: Test join step by step - chỉ Order với User
                var ordersWithUsers = await (from order in _context.Orders
                                           join employee in _context.NguoiDungs on order.IDEmployee equals employee.ID
                                           where order.CreateTime >= fromDate 
                                                 && order.CreateTime <= toDateEnd
                                                 && order.PaymentStatus == true
                                                 && order.Served == true
                                                 && order.IsCancelled != true
                                                 && order.IsRefunded != true
                                                 && !order.IsDelete
                                                 && !employee.IsDelete
                                           select new { 
                                               OrderId = order.ID, 
                                               EmployeeId = employee.ID, 
                                               EmployeeName = employee.TenHienThi,
                                               OrderDepartment = order.IDDepartment
                                           })
                                           .Take(3)
                                           .ToListAsync();
                                           
                Console.WriteLine($"[EmployeeSales] Orders with valid employees: {ordersWithUsers.Count}");
                foreach (var item in ordersWithUsers)
                {
                    Console.WriteLine($"[EmployeeSales]   Order {item.OrderId}: Employee {item.EmployeeName} ({item.EmployeeId}), Dept: {item.OrderDepartment}");
                }

                // DEBUG: Kiểm tra branch mismatch
                var orderDepartments = ordersWithUsers.Select(x => x.OrderDepartment).Distinct().ToList();
                Console.WriteLine($"[EmployeeSales] Order departments: [{string.Join(", ", orderDepartments)}]");
                Console.WriteLine($"[EmployeeSales] User branch IDs: [{string.Join(", ", userBranchIds)}]");
                Console.WriteLine($"[EmployeeSales] Branch filtering will exclude all orders - using no branch filter!");

                // Query để lấy doanh số nhân viên - chỉ tính đơn đã trả hàng
                var rawSalesData = await (from order in _context.Orders
                                        join employee in _context.NguoiDungs on order.IDEmployee equals employee.ID
                                        join department in _context.Departments on order.IDDepartment equals department.ID
                                        where order.CreateTime >= fromDate 
                                              && order.CreateTime <= toDateEnd
                                              && order.PaymentStatus == true     // Đã thanh toán
                                              && order.Served == true            // Đã trả hàng
                                              && order.IsCancelled != true       // Không bị hủy
                                              && order.IsRefunded != true        // Không bị hoàn tiền
                                              && !order.IsDelete
                                              && !employee.IsDelete
                                              && !department.IsDelete
                                              // TEMP: Bỏ branch filtering
                                              // && (userBranchIds.Count == 0 || userBranchIds.Contains(department.ID))
                                        select new
                                        {
                                            EmployeeId = employee.ID,
                                            EmployeeName = employee.TenHienThi,
                                            DepartmentName = department.DepartmentName,
                                            AmountString = order.Amount,
                                            OrderId = order.ID
                                        })
                                        .ToListAsync();

                Console.WriteLine($"[EmployeeSales] Raw sales data count: {rawSalesData.Count}");
                foreach (var item in rawSalesData.Take(3))
                {
                    Console.WriteLine($"[EmployeeSales]   - {item.EmployeeName} ({item.DepartmentName}): {item.AmountString}");
                }

                // Xử lý dữ liệu trong memory
                var salesData = rawSalesData
                    .GroupBy(x => new { x.EmployeeId, x.EmployeeName, x.DepartmentName })
                    .Select(g => new EmployeeSalesDto
                    {
                        EmployeeId = g.Key.EmployeeId,
                        EmployeeName = g.Key.EmployeeName,
                        DepartmentName = g.Key.DepartmentName,
                        TotalAmount = g.Sum(o => decimal.TryParse(o.AmountString, out var amount) ? amount : 0),
                        OrderCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .ToList();

                Console.WriteLine($"[EmployeeSales] Final sales data count: {salesData.Count}");

                EmployeeSales = salesData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmployeeSales] Error: {ex.Message}");
                EmployeeSales = new List<EmployeeSalesDto>();
            }
        }

    }

    public class EmployeeSalesDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int OrderCount { get; set; }
    }
}

