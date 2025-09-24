using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderModel = QRB.Models.Order;

namespace QRB.Pages.Order
{
    public class DeleteOrderModel : PageModel
    {
        private readonly QRBDbContext _context;
        private readonly ILogger<DeleteOrderModel> _logger;
        
        public DeleteOrderModel(QRBDbContext context, ILogger<DeleteOrderModel> logger)
        {
            _context = context;
            _logger = logger;
        }


        public class DeleteOrderRequest
        {
            public string? id { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DeleteOrderRequest? req = null;
            try
            {
                string body;
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }
                req = System.Text.Json.JsonSerializer.Deserialize<DeleteOrderRequest>(body);
                if (req == null || string.IsNullOrEmpty(req.id))
                {
                    return new JsonResult(new { success = false, message = "Thiếu id." });
                }
                if (!Guid.TryParse(req.id, out Guid orderId))
                {
                    return new JsonResult(new { success = false, message = "Id không hợp lệ." });
                }
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy hóa đơn." });
                }

                // SECURITY: Chỉ cho phép xóa đơn hàng chưa thanh toán
                if (order.PaymentStatus == true)
                {
                    _logger.LogWarning("Attempt to delete paid order {OrderId} with code {OrderCode}. Payment Date: {PaymentDate}", 
                        orderId, order.OrderCode, order.PaymentDate);
                        
                    return new JsonResult(new { 
                        success = false, 
                        message = "Không thể xóa đơn hàng đã thanh toán. Chỉ được phép xóa những đơn hàng chưa thanh toán." 
                    });
                }

                // Kiểm tra thêm: đơn hàng đã được xóa trước đó chưa
                if (order.IsDelete)
                {
                    _logger.LogWarning("Attempt to delete already deleted order {OrderId} with code {OrderCode}", 
                        orderId, order.OrderCode);
                        
                    return new JsonResult(new { 
                        success = false, 
                        message = "Đơn hàng này đã được xóa trước đó." 
                    });
                }

                _logger.LogInformation("Deleting unpaid order {OrderId} with code {OrderCode}. Created: {CreateTime}", 
                    orderId, order.OrderCode, order.CreateTime);

                // Xóa chi tiết đơn hàng trước
                var orderDetails = _context.OrderDetails.Where(od => od.IDOrder == orderId);
                _context.OrderDetails.RemoveRange(orderDetails);

                // Xóa đơn hàng
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully deleted order {OrderId} with code {OrderCode}", 
                    orderId, order.OrderCode);
                    
                return new JsonResult(new { success = true, message = "Xóa đơn hàng thành công!" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", req?.id);
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra khi xóa đơn hàng: " + ex.Message });
            }
        }
    }
}
