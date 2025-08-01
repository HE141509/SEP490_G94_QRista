-- Script cập nhật dữ liệu NguoiDung với thông tin mặc định
-- Chạy sau khi đã thực hiện CreateAuthorizationTables.sql

-- Cập nhật VaiTro cho người dùng hiện tại nếu NULL
UPDATE [dbo].[NguoiDung] 
SET [VaiTro] = 'Staff' 
WHERE [VaiTro] IS NULL OR [VaiTro] = '';

-- Cập nhật TrangThaiHoatDong mặc định là true
UPDATE [dbo].[NguoiDung] 
SET [TrangThaiHoatDong] = 1 
WHERE [TrangThaiHoatDong] IS NULL;

-- Tạo tài khoản admin mặc định nếu chưa có
IF NOT EXISTS (SELECT * FROM [NguoiDung] WHERE [TenNguoiDung] = 'admin')
BEGIN
    DECLARE @AdminBranchId UNIQUEIDENTIFIER;
    SELECT TOP 1 @AdminBranchId = [ID] FROM [ChiNhanh];
    
    IF @AdminBranchId IS NOT NULL
    BEGIN
        INSERT INTO [NguoiDung] (
            [ID], [TenNguoiDung], [MatKhau], [VaiTro], [TenHienThi], 
            [Email], [TrangThaiHoatDong], [IDChiNhanh], [IsDelete], [CreateTime]
        ) VALUES (
            NEWID(), 
            'admin', 
            'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', -- mật khẩu: 123456
            'Admin',
            'Quản trị viên',
            'admin@qrb.com',
            1,
            @AdminBranchId,
            0,
            GETDATE()
        );
    END
END

-- Tạo tài khoản manager mặc định nếu chưa có
IF NOT EXISTS (SELECT * FROM [NguoiDung] WHERE [TenNguoiDung] = 'manager')
BEGIN
    DECLARE @ManagerBranchId UNIQUEIDENTIFIER;
    SELECT TOP 1 @ManagerBranchId = [ID] FROM [ChiNhanh];
    
    IF @ManagerBranchId IS NOT NULL
    BEGIN
        INSERT INTO [NguoiDung] (
            [ID], [TenNguoiDung], [MatKhau], [VaiTro], [TenHienThi], 
            [Email], [TrangThaiHoatDong], [IDChiNhanh], [IsDelete], [CreateTime]
        ) VALUES (
            NEWID(), 
            'manager', 
            'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', -- mật khẩu: 123456
            'Manager',
            'Quản lý',
            'manager@qrb.com',
            1,
            @ManagerBranchId,
            0,
            GETDATE()
        );
    END
END

-- Phân quyền cho Admin (tất cả quyền)
INSERT INTO [RolePermissions] ([Id], [RoleId], [PermissionId], [GrantedAt])
SELECT 
    NEWID(),
    'admin-role',
    p.[Id],
    GETDATE()
FROM [Permissions] p
WHERE NOT EXISTS (
    SELECT 1 FROM [RolePermissions] rp 
    WHERE rp.[RoleId] = 'admin-role' AND rp.[PermissionId] = p.[Id]
);

-- Phân quyền cho Manager
DECLARE @ManagerPermissions TABLE (PermissionId NVARCHAR(450));
INSERT INTO @ManagerPermissions VALUES 
    ('product-management'), ('order-management'), ('inventory-management'),
    ('promotion-management'), ('reporting'), ('customer-management'),
    ('order-create'), ('order-view'), ('menu-view'), ('payment-management');

INSERT INTO [RolePermissions] ([Id], [RoleId], [PermissionId], [GrantedAt])
SELECT 
    NEWID(),
    'manager-role',
    mp.PermissionId,
    GETDATE()
FROM @ManagerPermissions mp
WHERE NOT EXISTS (
    SELECT 1 FROM [RolePermissions] rp 
    WHERE rp.[RoleId] = 'manager-role' AND rp.[PermissionId] = mp.PermissionId
);

-- Phân quyền cho Staff
DECLARE @StaffPermissions TABLE (PermissionId NVARCHAR(450));
INSERT INTO @StaffPermissions VALUES 
    ('order-create'), ('order-view'), ('menu-view'), ('customer-management');

INSERT INTO [RolePermissions] ([Id], [RoleId], [PermissionId], [GrantedAt])
SELECT 
    NEWID(),
    'staff-role',
    sp.PermissionId,
    GETDATE()
FROM @StaffPermissions sp
WHERE NOT EXISTS (
    SELECT 1 FROM [RolePermissions] rp 
    WHERE rp.[RoleId] = 'staff-role' AND rp.[PermissionId] = sp.PermissionId
);

-- Phân quyền cho Cashier
DECLARE @CashierPermissions TABLE (PermissionId NVARCHAR(450));
INSERT INTO @CashierPermissions VALUES 
    ('order-view'), ('menu-view'), ('cashier-operations'), ('payment-management');

INSERT INTO [RolePermissions] ([Id], [RoleId], [PermissionId], [GrantedAt])
SELECT 
    NEWID(),
    'cashier-role',
    cp.PermissionId,
    GETDATE()
FROM @CashierPermissions cp
WHERE NOT EXISTS (
    SELECT 1 FROM [RolePermissions] rp 
    WHERE rp.[RoleId] = 'cashier-role' AND rp.[PermissionId] = cp.PermissionId
);

PRINT 'Đã cập nhật dữ liệu người dùng và phân quyền thành công!';

-- Hiển thị thống kê
SELECT 'Roles' as TableName, COUNT(*) as RecordCount FROM [Roles]
UNION ALL
SELECT 'Permissions', COUNT(*) FROM [Permissions]
UNION ALL
SELECT 'RolePermissions', COUNT(*) FROM [RolePermissions]
UNION ALL
SELECT 'NguoiDung (Active)', COUNT(*) FROM [NguoiDung] WHERE [IsDelete] = 0;
