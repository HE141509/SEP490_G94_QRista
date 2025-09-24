# Bảo mật thanh toán VNPAY - Các cải thiện đã thực hiện

## Vấn đề ban đầu
Khách hàng/kẻ tấn công có thể giả lập trạng thái đã thanh toán bằng cách tạo link return giả từ sandbox VNPAY.

## Các biện pháp bảo mật đã triển khai

### 1. **Xác thực chữ ký HMAC-SHA512**
- ✅ Đã có sẵn: Kiểm tra `vnp_SecureHash` để đảm bảo dữ liệu không bị giả mạo
- ✅ Cải thiện: Thông báo lỗi rõ ràng hơn khi phát hiện chữ ký không hợp lệ

### 2. **Bảo mật Secret Key**
- ✅ **MỚI**: Di chuyển secret key từ hardcode sang `appsettings.json`
- ✅ **MỚI**: Có thể cấu hình qua environment variables cho production
- ⚠️ **LƯU Ý**: Cần thay đổi secret key thực tế trong production

### 3. **Validation IP nguồn**
- ✅ **MỚI**: Chỉ chấp nhận request từ IP được phép
- ✅ **MỚI**: Whitelist IP của VNPAY (cần cập nhật theo documentation mới nhất)
- ✅ **MỚI**: Cho phép localhost trong môi trường development

### 4. **Chống Replay Attack**
- ✅ **MỚI**: Lưu các transaction ID đã xử lý
- ✅ **MỚI**: Kiểm tra đơn hàng đã được thanh toán chưa trước khi cập nhật
- ✅ **MỚI**: Tự động cleanup các transaction cũ sau 24h

### 5. **Rate Limiting**
- ✅ **MỚI**: Giới hạn tối đa 10 request/phút từ cùng một IP
- ✅ **MỚI**: Sử dụng MemoryCache để tracking

### 6. **Validation Timestamp**
- ✅ **MỚI**: Kiểm tra thời gian thanh toán trong khoảng hợp lệ
- ✅ **MỚI**: Từ chối request quá cũ (>1h) hoặc trong tương lai

### 7. **Logging và Monitoring**
- ✅ **MỚI**: Log tất cả các attempt xác thực
- ✅ **MỚI**: Log cảnh báo khi phát hiện request bất thường
- ✅ **MỚI**: Log thông tin transaction thành công

## Files đã thay đổi

1. **`Services/VnpaySecurityService.cs`** - Service bảo mật mới
2. **`Pages/Vnpay/VnpayReturn.cshtml.cs`** - Tích hợp security service
3. **`Helpers/VnpayHelper.cs`** - Load secret từ configuration
4. **`Program.cs`** - Đăng ký services và cấu hình
5. **`appsettings.json`** - Thêm cấu hình VNPAY

## Hướng dẫn triển khai Production

### Bước 1: Cấu hình Secret Key
```json
{
  "VnpaySettings": {
    "HashSecret": "YOUR_REAL_PRODUCTION_SECRET",
    "TmnCode": "YOUR_PRODUCTION_TMN_CODE",
    "ReturnUrl": "https://yourdomain.com/VnpayReturn",
    "PaymentUrl": "https://pay.vnpay.vn/vpcpay.html"
  }
}
```

### Bước 2: Cập nhật IP whitelist
Trong `VnpaySecurityService.cs`, cập nhật `_allowedIPs` với IP ranges chính thức của VNPAY.

### Bước 3: Environment Variables (Khuyến nghị)
```bash
VNPAY_HASH_SECRET=your_secret_key
VNPAY_TMN_CODE=your_tmn_code
```

### Bước 4: SSL/TLS
Đảm bảo website chạy HTTPS trong production.

## Kiểm tra bảo mật

### Test Case 1: Fake Return URL
```
http://localhost:5233/VnpayReturn?vnp_ResponseCode=00&orderId=123
```
**Kết quả mong đợi**: Bị từ chối do thiếu chữ ký hợp lệ

### Test Case 2: Replay Attack
Gửi cùng một request hợp lệ 2 lần.
**Kết quả mong đợi**: Lần thứ 2 bị từ chối

### Test Case 3: Rate Limiting
Gửi >10 requests trong 1 phút từ cùng IP.
**Kết quả mong đợi**: Bị từ chối sau request thứ 10

## Monitoring và Alerting

Theo dõi các log sau trong production:
- `Invalid source IP`
- `Replay attack detected`
- `Rate limit exceeded`
- `Invalid signature detected`

## Lưu ý quan trọng

⚠️ **SECURITY WARNING**: 
- Secret key trong `appsettings.json` chỉ dành cho development
- Production PHẢI sử dụng Azure Key Vault hoặc environment variables
- Định kỳ rotate secret keys
- Monitor logs để phát hiện attack pattern

✅ **Với các cải thiện này, hệ thống đã được bảo vệ tốt hơn nhiều trước các cuộc tấn công giả mạo thanh toán.**
