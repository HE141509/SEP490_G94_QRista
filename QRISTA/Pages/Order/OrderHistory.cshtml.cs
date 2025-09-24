using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Pages.Order
{
    public class OrderHistoryModel : PageModel
    {
        private readonly QRBDbContext _context;

        public OrderHistoryModel(QRBDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? Phone { get; set; }

        public List<OrderHistoryItem> Orders { get; set; } = new List<OrderHistoryItem>();
        public string CustomerName { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Phone))
            {
                return RedirectToPage("/Menu/Menu");
            }

            try
            {
                // Tìm khách hàng theo số điện thoại
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Phone == Phone && !c.IsDelete);

                if (customer != null)
                {
                    CustomerName = customer.CustomerName ?? "Khách hàng";

                    // Lấy danh sách đơn hàng của khách hàng
                    var orders = await _context.Orders
                        .Where(o => o.IDCustomer == customer.ID && o.PaymentStatus == true)
                        .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                        .Include(o => o.Department)
                        .OrderByDescending(o => o.CreateTime)
                        .ToListAsync();

                    Orders = orders.Select(o => new OrderHistoryItem
                    {
                        OrderId = o.ID,
                        OrderCode = o.OrderCode ?? "",
                        CreateTime = o.CreateTime,
                        TotalAmount = decimal.TryParse(o.Amount, out var amount) ? amount : 0,
                        BranchName = o.Department?.DepartmentName ?? "",
                        ItemCount = o.OrderDetails.Count,
                        Items = o.OrderDetails.Select(od => new OrderHistoryDetail
                        {
                            ProductName = od.Product?.TenSanPham ?? "",
                            Quantity = od.Quantity,
                            Price = decimal.TryParse(od.Price, out var price) ? price : 0,
                            Total = decimal.TryParse(od.Total, out var total) ? total : 0
                        }).ToList(),
                        PaymentDate = o.PaymentDate,
                        Served = o.Served ?? false
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                Console.WriteLine($"Error loading order history: {ex.Message}");
            }

            return Page();
        }

        // API lấy chi tiết hóa đơn cho khách hàng
        public async Task<JsonResult> OnGetGetOrderDetailAsync(Guid id)
        {
            try
            {
                // Lấy thông tin hóa đơn
                var order = await _context.Orders
                    .Include(d => d.Customer)
                    .Include(d => d.Department)
                    .Where(d => d.ID == id && !d.IsDelete && d.PaymentStatus == true)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy hóa đơn" });
                }

                // Kiểm tra xem đơn hàng có thuộc về số điện thoại đang xem không
                if (!string.IsNullOrEmpty(Phone) && order.Customer?.Phone != Phone)
                {
                    return new JsonResult(new { success = false, message = "Không có quyền xem đơn hàng này" });
                }

                // Lấy chi tiết hóa đơn
                var orderDetails = await _context.OrderDetails
                    .Where(ct => ct.IDOrder == id && !ct.IsDelete)
                    .Include(ct => ct.Product)
                    .Select(ct => new
                    {
                        tenSanPham = ct.Product!.TenSanPham,
                        soLuong = ct.Quantity,
                        donGia = ct.Price,
                        thanhTien = ct.Total
                    })
                    .ToListAsync();

                var result = new
                {
                    success = true,
                    data = new
                    {
                        id = order.ID,
                        maDonHang = order.OrderCode,
                        tenKhachHang = order.Customer?.CustomerName ?? "",
                        sdtKhachHang = order.Customer?.Phone ?? "",
                        tenChiNhanh = order.Department?.DepartmentName ?? "",
                        tongTien = order.Amount,
                        createTime = order.CreateTime.ToString("dd/MM/yyyy HH:mm"),
                        orderDetails = orderDetails
                    }
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi lấy chi tiết hóa đơn: " + ex.Message });
            }
        }

        public async Task<JsonResult> OnPostSetCartSessionAsync([FromBody] SetCartSessionRequest request)
        {
            try
            {
                // Lưu cart data vào session
                HttpContext.Session.SetString("qrb_cart_data", request.CartData);
                
                // Lưu số điện thoại khách hàng vào session
                if (!string.IsNullOrEmpty(request.Phone))
                {
                    HttpContext.Session.SetString("PhoneNumber", request.Phone);
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi lưu giỏ hàng: " + ex.Message });
            }
        }
    }

    public class SetCartSessionRequest
    {
        public string CartData { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    public class OrderHistoryItem
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime CreateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string BranchName { get; set; } = "";
        public int ItemCount { get; set; }
        public List<OrderHistoryDetail> Items { get; set; } = new List<OrderHistoryDetail>();
        public DateTime? PaymentDate { get; set; }
        public bool Served { get; set; }
    }

    public class OrderHistoryDetail
    {
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
