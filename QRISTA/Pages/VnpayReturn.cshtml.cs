using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using QRB.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QRB.Pages
{
    public class VnpayReturnModel : PageModel
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? OrderId { get; set; }
        // Lưu toàn bộ tham số trả về từ VNPAY (bao gồm cả các trường không phải vnp_)
        public Dictionary<string, string> AllParams { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> VnpParams { get; set; } = new Dictionary<string, string>();
        public void OnGet()
        {
            var query = HttpContext.Request.Query;
            // Lưu tất cả tham số trả về (bao gồm cả các trường không phải vnp_)
            foreach (var key in query.Keys)
            {
                var value = query[key].ToString();
                if (!string.IsNullOrEmpty(value))
                    AllParams[key] = value;
                if (key.StartsWith("vnp_") && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    VnpParams[key] = value;
                }
            }
            // Lấy secure hash
            string? vnp_SecureHash = query.ContainsKey("vnp_SecureHash") ? query["vnp_SecureHash"].ToString() : null;
            // Sắp xếp params theo key và build chuỗi hash đúng chuẩn VNPAY
            var ordered = VnpParams.OrderBy(x => x.Key);
            StringBuilder data = new StringBuilder();
            foreach (var kv in ordered)
            {
                if (data.Length > 0) data.Append('&');
                data.Append(kv.Key + "=" + kv.Value);
            }
            // Lấy secret key
            string vnp_HashSecret = VnpayHelper.Vnp_HashSecret;
            string checkHash = VnpayHelper.HmacSHA512(vnp_HashSecret, data.ToString());
            // Kiểm tra checksum
            if (!string.IsNullOrEmpty(vnp_SecureHash) && vnp_SecureHash.Equals(checkHash, System.StringComparison.InvariantCultureIgnoreCase))
            {
                // Xác thực thành công
                if (VnpParams.ContainsKey("vnp_ResponseCode") && VnpParams["vnp_ResponseCode"] == "00")
                {
                    IsSuccess = true;
                    Message = "Thanh toán thành công!";
                }
                else
                {
                    IsSuccess = false;
                    Message = $"Thanh toán thất bại. Mã lỗi: {VnpParams["vnp_ResponseCode"]}";
                }
            }
            else
            {
                IsSuccess = false;
                Message = "Sai chữ ký xác thực!";
            }
            // Lấy orderId nếu có
            if (query.ContainsKey("orderId"))
                OrderId = query["orderId"].ToString();
            else if (VnpParams.ContainsKey("vnp_TxnRef"))
                OrderId = VnpParams["vnp_TxnRef"];
        }
    }
}
