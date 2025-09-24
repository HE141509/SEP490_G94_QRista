-- Script kiểm tra quyền của user hiện tại
-- Thay 'your_username' bằng tên đăng nhập thực tế

DECLARE @Username NVARCHAR(50) = 'admin'; -- Thay đổi tên user tại đây

SELECT 
    u.TenNguoiDung as Username,
    u.VaiTro as UserRole,
    r.Name as RoleName,
    p.Name as PermissionName,
    p.Description as PermissionDescription,
    p.Module as PermissionModule
FROM NguoiDung u
LEFT JOIN Roles r ON u.VaiTro = r.Name
LEFT JOIN RolePermissions rp ON r.Id = rp.RoleId
LEFT JOIN Permissions p ON rp.PermissionId = p.Id
WHERE u.TenNguoiDung = @Username
ORDER BY p.Module, p.Name;

-- Kiểm tra xem có quyền order-management không
SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM NguoiDung u
            JOIN Roles r ON u.VaiTro = r.Name
            JOIN RolePermissions rp ON r.Id = rp.RoleId
            JOIN Permissions p ON rp.PermissionId = p.Id
            WHERE u.TenNguoiDung = @Username 
            AND p.Id IN ('order-management', 'Full Invoices', 'order-delete')
        ) THEN 'CÓ QUYỀN loại bỏ hóa đơn'
        ELSE 'KHÔNG CÓ QUYỀN loại bỏ hóa đơn'
    END as PermissionStatus;
