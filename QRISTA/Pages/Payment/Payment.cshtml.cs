using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using QRB.Helpers;
using System;
using System.Net;

namespace QRB.Pages.Payment
{
    public class PaymentModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Amount { get; set; }
        public string? PaymentUrl { get; set; }
        public void OnGet()
        {
            // Đảm bảo số tiền > 0 và là số nguyên
            if (Amount < 1000) Amount = 1000;

            // Thông tin cấu hình VNPAY demo (bạn cần thay bằng thông tin thật khi live)
            string tmnCode = "PT8AZLP3"; // Mã merchant thật của bạn (sandbox)
            string hashSecret = "PWSHBSJCVDL54CGNA6C1F55ZZIPV6XP2"; // Chuỗi bí mật thật (sandbox)
            string orderInfo = "Thanh toan don hang QRista Cafe";
            string orderType = "billpayment"; // Nên dùng mã chuẩn VNPAY

            // txnRef nên là mã đơn hàng thực tế, nếu chưa có thì tạo theo timestamp để không trùng
            string txnRef = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            // Truyền mã đơn hàng (txnRef) vào returnUrl để callback nhận diện đơn hàng
            string returnUrl = $"https://localhost:5233/VnpayReturn?orderId={txnRef}";
            string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            PaymentUrl = VnpayHelper.CreateVnpayPaymentUrl(
                tmnCode,
                hashSecret,
                Amount,
                orderInfo,
                orderType,
                returnUrl,
                clientIp,
                txnRef
            );
        }
    }
}
