using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using OrderModel = QRB.Models.Order;

namespace QRB.Pages.Order
{
    public class OrderDetailModel : PageModel
    {
        private readonly QRBDbContext _context;
        
        public OrderDetailModel(QRBDbContext context)
        {
            _context = context;
        }
        
        public OrderModel? Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DepartmentName { get; set; }
        public bool OrderFound { get; set; } = false;
        public string CurrentBranchName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

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

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Index");
            }

            // Kiểm tra quyền truy cập
            if (!HasPermission("View Orders") && !HasPermission("Full Orders"))
            {
                return Redirect($"/AccessDenied?permission=View Orders&module=Orders");
            }

            DisplayName = HttpContext.Session.GetString("DisplayName") ?? "";
            CurrentBranchName = HttpContext.Session.GetString("BranchName") ?? "";

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                // Tìm đơn hàng theo OrderCode
                Order = await _context.Orders
                    .Include(o => o.Department)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.OrderCode == id);

                if (Order == null)
                {
                    return NotFound();
                }

                OrderFound = true;

                // Lấy thông tin khách hàng
                if (Order.Customer != null)
                {
                    CustomerName = Order.Customer.CustomerName;
                    CustomerPhone = Order.Customer.Phone;
                }

                // Lấy thông tin chi nhánh
                if (Order.Department != null)
                {
                    DepartmentName = Order.Department.DepartmentName;
                }

                // Lấy chi tiết đơn hàng
                OrderDetails = await _context.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.ProductType)
                    .Where(od => od.IDOrder == Order.ID && !od.IsDelete)
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error loading order detail: {ex.Message}");
                return NotFound();
            }
        }
    }
}
