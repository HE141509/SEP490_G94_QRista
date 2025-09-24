-- Script thêm quyền xóa đơn hàng vào database
-- Chạy script này trong SQL Server Management Studio hoặc Azure Data Studio

-- Thêm quyền mới cho việc xóa đơn hàng
INSERT INTO [Permissions] ([Id], [Name], [Description], [Module], [CreatedAt])
VALUES 
    ('order-delete', 'Delete Orders', 'Xóa và loại bỏ đơn hàng', 'Order Management', GETDATE());

-- Cấp quyền cho role Admin (đã có tất cả quyền)
INSERT INTO [RolePermissions] ([Id], [RoleId], [PermissionId], [GrantedAt])
SELECT 
    NEWID(),
    'admin-role',
    'order-delete',
    GETDATE()
WHERE NOT EXISTS (
    SELECT 1 FROM [RolePermissions] 
    WHERE [RoleId] = 'admin-role' AND [PermissionId] = 'order-delete'
);

-- Cấp quyền cho role Manager
INSERT INTO [RolePermissions] ([Id], [RoleId], [PermissionId], [GrantedAt])
SELECT 
    NEWID(),
    'manager-role',
    'order-delete',
    GETDATE()
WHERE NOT EXISTS (
    SELECT 1 FROM [RolePermissions] 
    WHERE [RoleId] = 'manager-role' AND [PermissionId] = 'order-delete'
);

PRINT 'Đã thêm quyền order-delete và cấp cho Admin, Manager';

-- Kiểm tra kết quả
SELECT r.Name as RoleName, p.Name as PermissionName, p.Description
FROM RolePermissions rp
JOIN Roles r ON rp.RoleId = r.Id
JOIN Permissions p ON rp.PermissionId = p.Id
WHERE p.Id = 'order-delete';
