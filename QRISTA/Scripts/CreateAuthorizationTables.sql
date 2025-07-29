-- Script để tạo các bảng cho hệ thống phân quyền
-- Chạy script này trong SQL Server Management Studio

-- Tạo bảng Roles
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
ELSE
BEGIN
    -- Thêm cột IsActive nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Roles') AND name = 'IsActive')
    BEGIN
        ALTER TABLE [dbo].[Roles] ADD [IsActive] BIT NOT NULL DEFAULT 1;
    END
END

-- Tạo bảng Permissions
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Permissions' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Permissions] (
        [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(200) NULL,
        [Module] NVARCHAR(50) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END

-- Tạo bảng RolePermissions
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RolePermissions' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[RolePermissions] (
        [Id] NVARCHAR(450) NOT NULL PRIMARY KEY,
        [RoleId] NVARCHAR(450) NOT NULL,
        [PermissionId] NVARCHAR(450) NOT NULL,
        [GrantedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]),
        CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions]([Id])
    );
END

-- Thêm các cột mới vào bảng NguoiDung nếu chưa có
IF EXISTS (SELECT * FROM sysobjects WHERE name='NguoiDung' AND xtype='U')
BEGIN
    -- Thêm cột VaiTro
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NguoiDung') AND name = 'VaiTro')
    BEGIN
        ALTER TABLE [dbo].[NguoiDung] ADD [VaiTro] NVARCHAR(50) NULL;
        -- Cập nhật giá trị mặc định
        UPDATE [dbo].[NguoiDung] SET [VaiTro] = 'Staff' WHERE [VaiTro] IS NULL;
    END

    -- Thêm cột TrangThaiHoatDong
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NguoiDung') AND name = 'TrangThaiHoatDong')
    BEGIN
        ALTER TABLE [dbo].[NguoiDung] ADD [TrangThaiHoatDong] BIT NOT NULL DEFAULT 1;
    END

    -- Thêm cột Email
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NguoiDung') AND name = 'Email')
    BEGIN
        ALTER TABLE [dbo].[NguoiDung] ADD [Email] NVARCHAR(255) NULL;
    END
END

-- Thêm dữ liệu mẫu cho Roles
IF NOT EXISTS (SELECT * FROM [Roles] WHERE [Id] = 'admin-role')
BEGIN
    INSERT INTO [Roles] ([Id], [Name], [Description], [IsActive], [CreatedAt])
    VALUES 
        ('admin-role', 'Admin', 'Quản trị viên hệ thống', 1, GETDATE()),
        ('manager-role', 'Manager', 'Quản lý cửa hàng', 1, GETDATE()),
        ('staff-role', 'Staff', 'Nhân viên bán hàng', 1, GETDATE()),
        ('cashier-role', 'Cashier', 'Thu ngân', 1, GETDATE());
END

-- Thêm dữ liệu mẫu cho Permissions
IF NOT EXISTS (SELECT * FROM [Permissions] WHERE [Id] = 'user-management')
BEGIN
    INSERT INTO [Permissions] ([Id], [Name], [Description], [Module], [CreatedAt])
    VALUES 
        ('user-management', 'Quản lý người dùng', 'Thêm, sửa, xóa người dùng', 'User Management', GETDATE()),
        ('product-management', 'Quản lý sản phẩm', 'Quản lý menu và sản phẩm', 'Product Management', GETDATE()),
        ('order-management', 'Quản lý đơn hàng', 'Xem và xử lý đơn hàng', 'Order Management', GETDATE()),
        ('inventory-management', 'Quản lý kho', 'Quản lý tồn kho và nhập xuất', 'Inventory Management', GETDATE()),
        ('branch-management', 'Quản lý chi nhánh', 'Thêm, sửa, xóa chi nhánh', 'Branch Management', GETDATE()),
        ('promotion-management', 'Quản lý khuyến mãi', 'Tạo và quản lý ưu đãi', 'Promotion Management', GETDATE()),
        ('reporting', 'Báo cáo thống kê', 'Xem báo cáo và thống kê', 'Reporting', GETDATE()),
        ('system-config', 'Cấu hình hệ thống', 'Cấu hình tham số hệ thống', 'System Configuration', GETDATE()),
        ('payment-management', 'Quản lý thanh toán', 'Xử lý thanh toán và hoàn tiền', 'Payment Management', GETDATE()),
        ('customer-management', 'Quản lý khách hàng', 'Quản lý thông tin khách hàng', 'Customer Management', GETDATE()),
        ('order-create', 'Tạo đơn hàng', 'Tạo đơn hàng mới', 'Order Management', GETDATE()),
        ('order-view', 'Xem đơn hàng', 'Xem thông tin đơn hàng', 'Order Management', GETDATE()),
        ('menu-view', 'Xem menu', 'Xem danh sách sản phẩm', 'Product Management', GETDATE()),
        ('cashier-operations', 'Thao tác thu ngân', 'Thu tiền và in hóa đơn', 'Payment Management', GETDATE());
END

PRINT 'Đã tạo/cập nhật database schema cho hệ thống phân quyền thành công!';
