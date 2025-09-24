# Cải thiện bảo mật thanh toán VNPAY

## Vấn đề hiện tại:
- Secret key được hardcode trong source code
- Không có validation IP nguồn
- Không có protection chống replay attack
- Không có rate limiting

## Giải pháp đề xuất:

### 1. Bảo mật Secret Key
- Di chuyển secret key vào appsettings.json hoặc environment variables
- Sử dụng Azure Key Vault hoặc tương tự cho production

### 2. Validation IP nguồn
- Chỉ chấp nhận request từ IP của VNPAY
- Whitelist các IP được phép

### 3. Chống Replay Attack
- Lưu các transaction ID đã xử lý
- Kiểm tra timestamp của request
- Đảm bảo mỗi transaction chỉ được xử lý 1 lần

### 4. Rate Limiting
- Giới hạn số lượng request từ cùng IP
- Giới hạn số lần retry cho cùng transaction

### 5. Logging và Monitoring
- Log tất cả các attempt xác thực
- Alert khi có request bất thường
- Monitor các pattern tấn công
