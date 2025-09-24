-- Câu lệnh SQL để thêm các trường Cancel/Refund vào bảng Order
-- Chạy trực tiếp trong SQL Server Management Studio

USE [QRB]; -- Thay tên database nếu khác
GO

-- Thêm các cột Cancel/Refund vào bảng Order
ALTER TABLE [Order] ADD 
    [CancelReason] nvarchar(500) NULL,
    [CancelledByUserId] uniqueidentifier NULL,
    [CancelledDate] datetime2 NULL,
    [IsCancelled] bit NULL,
    [IsRefunded] bit NULL,
    [RefundAmount] nvarchar(255) NULL,
    [RefundApprovedByUserId] uniqueidentifier NULL,
    [RefundDate] datetime2 NULL,
    [RefundReason] nvarchar(500) NULL;
GO

-- Kiểm tra cấu trúc bảng sau khi thêm
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Order' 
  AND COLUMN_NAME IN ('CancelReason', 'CancelledByUserId', 'CancelledDate', 'IsCancelled', 'IsRefunded', 'RefundAmount', 'RefundApprovedByUserId', 'RefundDate', 'RefundReason')
ORDER BY COLUMN_NAME;
GO

-- Cập nhật giá trị mặc định cho các cột boolean (tùy chọn)
UPDATE [Order] 
SET 
    [IsCancelled] = 0,
    [IsRefunded] = 0
WHERE [IsCancelled] IS NULL OR [IsRefunded] IS NULL;
GO

PRINT 'Đã thêm thành công các trường Cancel/Refund vào bảng Order!';
