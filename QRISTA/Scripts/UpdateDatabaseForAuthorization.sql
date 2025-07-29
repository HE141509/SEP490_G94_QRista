-- Script để cập nhật database cho hệ thống phân quyền
-- Chạy script này trong SQL Server Management Studio

-- 1. Thêm các cột mới vào bảng NguoiDung (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NguoiDung' AND COLUMN_NAME = 'VaiTro')
BEGIN
    ALTER TABLE NguoiDung ADD VaiTro NVARCHAR(50) NOT NULL DEFAULT 'Staff';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NguoiDung' AND COLUMN_NAME = 'TrangThaiHoatDong')
BEGIN
    ALTER TABLE NguoiDung ADD TrangThaiHoatDong BIT NOT NULL DEFAULT 1;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NguoiDung' AND COLUMN_NAME = 'Email')
BEGIN
    ALTER TABLE NguoiDung ADD Email NVARCHAR(100) NULL;
END

-- 2. Tạo bảng Roles (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')
BEGIN
    CREATE TABLE Roles (
        Id NVARCHAR(450) PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL,
        Description NVARCHAR(200),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
    
    CREATE UNIQUE INDEX IX_Roles_Name ON Roles (Name);
END

-- 3. Tạo bảng Permissions (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Permissions')
BEGIN
    CREATE TABLE Permissions (
        Id NVARCHAR(450) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(200),
        Category NVARCHAR(50),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

-- 4. Tạo bảng RolePermissions (nếu chưa có)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RolePermissions')
BEGIN
    CREATE TABLE RolePermissions (
        Id NVARCHAR(450) PRIMARY KEY,
        RoleId NVARCHAR(450) NOT NULL,
        PermissionId NVARCHAR(450) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE,
        FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE
    );
    
    CREATE UNIQUE INDEX IX_RolePermissions_RoleId_PermissionId ON RolePermissions (RoleId, PermissionId);
END

-- 5. Cập nhật dữ liệu hiện có trong bảng NguoiDung
UPDATE NguoiDung 
SET VaiTro = CASE 
    WHEN TenNguoiDung = 'admin' THEN 'Admin'
    WHEN TenNguoiDung = 'manager' THEN 'Manager'
    ELSE 'Staff'
END
WHERE VaiTro IS NULL OR VaiTro = '';

UPDATE NguoiDung 
SET TrangThaiHoatDong = 1
WHERE TrangThaiHoatDong IS NULL;

-- 6. Thêm dữ liệu mẫu cho Roles
IF NOT EXISTS (SELECT * FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Id, Name, Description) VALUES 
    ('admin-role', 'Admin', 'Quản trị viên hệ thống'),
    ('manager-role', 'Manager', 'Quản lý cửa hàng'),
    ('staff-role', 'Staff', 'Nhân viên bán hàng'),
    ('cashier-role', 'Cashier', 'Thu ngân');
END

-- 7. Thêm dữ liệu mẫu cho Permissions
IF NOT EXISTS (SELECT * FROM Permissions WHERE Id = 'user-management')
BEGIN
    INSERT INTO Permissions (Id, Name, Description, Category) VALUES 
    ('user-management', N'Quản lý người dùng', N'Thêm, sửa, xóa người dùng', 'User Management'),
    ('product-management', N'Quản lý sản phẩm', N'Quản lý menu và sản phẩm', 'Product Management'),
    ('order-management', N'Quản lý đơn hàng', N'Xem và xử lý đơn hàng', 'Order Management'),
    ('inventory-management', N'Quản lý kho', N'Quản lý tồn kho và nhập xuất', 'Inventory Management'),
    ('branch-management', N'Quản lý chi nhánh', N'Thêm, sửa, xóa chi nhánh', 'Branch Management'),
    ('promotion-management', N'Quản lý khuyến mãi', N'Tạo và quản lý ưu đãi', 'Promotion Management'),
    ('reporting', N'Báo cáo thống kê', N'Xem báo cáo và thống kê', 'Reporting'),
    ('system-config', N'Cấu hình hệ thống', N'Cấu hình tham số hệ thống', 'System Configuration'),
    ('payment-management', N'Quản lý thanh toán', N'Xử lý thanh toán và hoàn tiền', 'Payment Management'),
    ('customer-management', N'Quản lý khách hàng', N'Quản lý thông tin khách hàng', 'Customer Management'),
    ('order-create', N'Tạo đơn hàng', N'Tạo đơn hàng mới', 'Order Management'),
    ('order-view', N'Xem đơn hàng', N'Xem thông tin đơn hàng', 'Order Management'),
    ('menu-view', N'Xem menu', N'Xem danh sách sản phẩm', 'Product Management'),
    ('cashier-operations', N'Thao tác thu ngân', N'Thu tiền và in hóa đơn', 'Payment Management');
END

-- 8. Gán quyền cho các vai trò
-- Admin có tất cả quyền
IF NOT EXISTS (SELECT * FROM RolePermissions WHERE RoleId = 'admin-role')
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId)
    SELECT NEWID(), 'admin-role', Id FROM Permissions;
END

-- Manager có một số quyền
IF NOT EXISTS (SELECT * FROM RolePermissions WHERE RoleId = 'manager-role')
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId) VALUES 
    (NEWID(), 'manager-role', 'product-management'),
    (NEWID(), 'manager-role', 'order-management'),
    (NEWID(), 'manager-role', 'inventory-management'),
    (NEWID(), 'manager-role', 'promotion-management'),
    (NEWID(), 'manager-role', 'reporting'),
    (NEWID(), 'manager-role', 'customer-management'),
    (NEWID(), 'manager-role', 'order-create'),
    (NEWID(), 'manager-role', 'order-view'),
    (NEWID(), 'manager-role', 'menu-view'),
    (NEWID(), 'manager-role', 'payment-management');
END

-- Staff có quyền cơ bản
IF NOT EXISTS (SELECT * FROM RolePermissions WHERE RoleId = 'staff-role')
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId) VALUES 
    (NEWID(), 'staff-role', 'order-create'),
    (NEWID(), 'staff-role', 'order-view'),
    (NEWID(), 'staff-role', 'menu-view'),
    (NEWID(), 'staff-role', 'customer-management');
END

-- Cashier có quyền thu ngân
IF NOT EXISTS (SELECT * FROM RolePermissions WHERE RoleId = 'cashier-role')
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId) VALUES 
    (NEWID(), 'cashier-role', 'order-view'),
    (NEWID(), 'cashier-role', 'menu-view'),
    (NEWID(), 'cashier-role', 'cashier-operations'),
    (NEWID(), 'cashier-role', 'payment-management');
END

PRINT 'Database updated successfully for Authorization system!';
