using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System.Threading.Tasks;
using System.Linq;

namespace QRB.Pages.Order
{
    public class DeleteOrderModel : PageModel
    {
        private readonly QRBDbContext _context;
        public DeleteOrderModel(QRBDbContext context)
        {
            _context = context;
        }


        public class DeleteOrderRequest
        {
            public string id { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                string body;
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }
                var req = System.Text.Json.JsonSerializer.Deserialize<DeleteOrderRequest>(body);
                if (req == null || string.IsNullOrEmpty(req.id))
                {
                    return new JsonResult(new { success = false, message = "Thiếu id." });
                }
                if (!Guid.TryParse(req.id, out Guid orderId))
                {
                    return new JsonResult(new { success = false, message = "Id không hợp lệ." });
                }
                var order = await _context.DonHangs.FindAsync(orderId);
                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy hóa đơn." });
                }
                _context.DonHangs.Remove(order);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
