# Hướng dẫn cập nhật Database cho hệ thống phân quyền

## Bước 1: Cập nhật schema database
1. Mở SQL Server Management Studio
2. Kết nối đến database QRB
3. Mở và chạy file `Scripts/CreateAuthorizationTables.sql`

## Bước 2: Cập nhật dữ liệu
1. Chạy file `Scripts/SeedAuthorizationData.sql`

## Bước 3: Kiểm tra dữ liệu
Sau khi chạy script, bạn sẽ có:
- **4 vai trò**: Admin, Manager, Staff, Cashier
- **14 quyền hạn** được nhóm theo module
- **Tài khoản admin mặc định**:
  - Username: `admin`
  - Password: `123456`
  - Email: `admin@qrb.com`

## Bước 4: Chạy ứng dụng
```bash
cd "c:\Users\Admin\Desktop\DuAn\qrb\C#"
dotnet run --urls "http://localhost:5233"
```

## Bước 5: Truy cập hệ thống phân quyền
- Dashboard phân quyền: http://localhost:5233/Authorization
- Quản lý người dùng: http://localhost:5233/Authorization/Users
- Quản lý vai trò: http://localhost:5233/Authorization/Roles
- Ma trận phân quyền: http://localhost:5233/Authorization/Permissions

## Lưu ý
- Tất cả mật khẩu mặc định là `123456`
- Admin có tất cả quyền hạn
- Manager có hầu hết quyền trừ quản lý người dùng và cấu hình hệ thống
- Staff chỉ có quyền cơ bản: tạo đơn hàng, xem menu, quản lý khách hàng
- Cashier chỉ có quyền thu ngân và xem đơn hàng

## Troubleshooting
Nếu gặp lỗi "Invalid column name", hãy đảm bảo đã chạy đầy đủ cả 2 script SQL.
