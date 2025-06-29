using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System;
using QRB.Helpers; // Đảm bảo namespace đúng với VnpayHelper

namespace QRB.Pages
{
    public class ReturnUrlModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? vnp_Amount { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_BankCode { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_OrderInfo { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_ResponseCode { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_TransactionNo { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_TxnRef { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_SecureHash { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_SecureHashType { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_PayDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_TransactionStatus { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_CardType { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_BankTranNo { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? vnp_SecureHashParam { get; set; }

        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        public void OnGet()
        {
            // Lấy tất cả query string
            var query = HttpContext.Request.Query;

            // Lấy tất cả tham số vnp_ và loại bỏ các key có giá trị null/rỗng, đồng thời bỏ vnp_SecureHash, vnp_SecureHashType
            var vnp_Params = new Dictionary<string, string>();
            foreach (var key in query.Keys)
            {
                if (key.StartsWith("vnp_") && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    var value = query[key].ToString();
                    if (!string.IsNullOrEmpty(value))
                        vnp_Params[key] = value;
                }
            }

            // Lấy secure hash
            string? vnp_SecureHash = query.ContainsKey("vnp_SecureHash") ? query["vnp_SecureHash"].ToString() : null;

            // Sắp xếp params theo key và build chuỗi hash đúng chuẩn VNPAY
            var ordered = vnp_Params.OrderBy(x => x.Key);
            StringBuilder data = new StringBuilder();
            foreach (var kv in ordered)
            {
                if (data.Length > 0) data.Append('&');
                data.Append(kv.Key + "=" + kv.Value);
            }

            // Lấy secret key từ cấu hình hoặc hardcode (sandbox)
            string vnp_HashSecret = VnpayHelper.Vnp_HashSecret; // Đảm bảo đã có biến này trong VnpayHelper
            string checkHash = VnpayHelper.HmacSHA512(vnp_HashSecret, data.ToString());

            if (!string.IsNullOrEmpty(vnp_SecureHash) && vnp_SecureHash.Equals(checkHash, StringComparison.InvariantCultureIgnoreCase))
            {
                // Xác thực thành công
                if (vnp_Params.ContainsKey("vnp_ResponseCode") && vnp_Params["vnp_ResponseCode"] == "00")
                {
                    IsSuccess = true;
                    Message = $"Giao dịch thành công. Mã đơn hàng: {vnp_Params["vnp_TxnRef"]}, Số tiền: {decimal.Parse(vnp_Params["vnp_Amount"]) / 100:0,0} VND.";
                }
                else
                {
                    IsSuccess = false;
                    Message = $"Giao dịch thất bại. Mã lỗi: {vnp_Params["vnp_ResponseCode"]}. Vui lòng thử lại hoặc liên hệ hỗ trợ.";
                }
            }
            else
            {
                IsSuccess = false;
                Message = "Dữ liệu không hợp lệ (sai checksum). Vui lòng không làm mới trang và liên hệ hỗ trợ.";
            }
        }
    }
}
