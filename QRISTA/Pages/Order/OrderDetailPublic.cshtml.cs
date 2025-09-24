using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using Microsoft.EntityFrameworkCore;
using OrderModel = QRB.Models.Order;

namespace QRB.Pages.Order
{
    public class OrderDetailPublicModel : PageModel
    {
        private readonly QRBDbContext _context;
        
        public OrderDetailPublicModel(QRBDbContext context)
        {
            _context = context;
        }
        
        public OrderModel? Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DepartmentName { get; set; }
        public bool OrderFound { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
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
