
-- Branches Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Branches', N'Quyền chi nhánh', N'Branches', GETDATE());

-- Invoices Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Invoices', N'Quyền hóa đơn', N'Invoices', GETDATE());

-- Customers Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Customers', N'Quyền khách hàng', N'Customers', GETDATE());

-- Products Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Products', N'Quyền sản phẩm', N'Products', GETDATE());

-- Product Groups Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Product Groups', N'Quyền nhóm sản phẩm', N'Product Groups', GETDATE());

-- Product Types Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Product Types', N'Quyền loại sản phẩm', N'Product Types', GETDATE());

-- Inventory Management Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Inventory', N'Quyền quản lý kho', N'Inventory Management', GETDATE());

-- Raw Materials Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Raw Materials', N'Quyền nguyên liệu', N'Raw Materials', GETDATE());

-- Product Warehouse Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Product Warehouse', N'Quyền kho sản phẩm', N'Product Warehouse', GETDATE());

-- Promotions Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Promotions', N'Quyền ưu đãi', N'Promotions', GETDATE());

-- Purchase Suggestions Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Purchase Suggestions', N'Quyền dđề xuất mua sắm', N'Purchase Suggestions', GETDATE());

-- User Management Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Users', N'Quyền người dùng', N'User Management', GETDATE());

-- Role Management Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Roles', N'Quyền vai trò', N'Role Management', GETDATE());

-- Authorization Matrix Module
INSERT INTO Permissions (Id, Name, Description, Module, CreatedAt) VALUES 
(NEWID(), N'Full Permission Matrix', N'Quyền ma trận phân quyền', N'Permission Matrix', GETDATE());

