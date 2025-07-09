using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace QRB.Helpers
{
    public static class VnpayHelper
    {
        public static string CreateVnpayPaymentUrl(
            string tmnCode,
            string hashSecret,
            int amount, // Số tiền VND, chưa nhân 100
            string orderInfo,
            string orderType,
            string returnUrl,
            string clientIp,
            string txnRef,
            string bankCode = null // optional
        )
        {
            var vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var vnp_Version = "2.1.1";
            var vnp_Command = "pay";
            var vnp_CurrCode = "VND";
            var vnp_Locale = "vn";
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_ExpireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

            var inputData = new SortedDictionary<string, string>
            {
                { "vnp_Version", vnp_Version },
                { "vnp_Command", vnp_Command },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", (amount * 100).ToString() },
                { "vnp_CurrCode", vnp_CurrCode },
                { "vnp_TxnRef", txnRef },
                { "vnp_OrderInfo", "thanhtoan" },
                { "vnp_OrderType", orderType },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_IpAddr", clientIp },
                { "vnp_CreateDate", vnp_CreateDate },
                { "vnp_ExpireDate", vnp_ExpireDate },
                { "vnp_Locale", vnp_Locale }
            };
            if (!string.IsNullOrEmpty(bankCode))
                inputData.Add("vnp_BankCode", bankCode);

            // Loại bỏ các tham số có giá trị null/rỗng (chuẩn VNPAY)
            var filteredData = inputData.Where(kvp => !string.IsNullOrEmpty(kvp.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Build data for hash (PHẢI encode key và value)
            var hashData = string.Join("&", filteredData.OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{System.Net.WebUtility.UrlEncode(kvp.Key)}={System.Net.WebUtility.UrlEncode(kvp.Value)}"));
            // Build query string (CÓ encode value)
            var query = string.Join("&", filteredData.OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{System.Net.WebUtility.UrlEncode(kvp.Key)}={System.Net.WebUtility.UrlEncode(kvp.Value)}"));

            // Tạo secure hash
            string vnp_SecureHash = HmacSHA512(hashSecret, hashData);
            var paymentUrl = $"{vnp_Url}?{query}&vnp_SecureHash={vnp_SecureHash}";
            return paymentUrl;
        }

        // Thêm biến public static để lấy secret key (có thể sửa lại cho phù hợp với cấu hình thực tế)
        public static string Vnp_HashSecret = "PWSHBSJCVDL54CGNA6C1F55ZZIPV6XP2"; // TODO: Thay bằng secret thực tế hoặc lấy từ cấu hình

        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}
