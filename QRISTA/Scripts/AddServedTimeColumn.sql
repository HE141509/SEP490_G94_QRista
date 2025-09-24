-- Thêm cột ServedTime vào bảng Order để lưu thời gian khi ấn nút "trả hàng"
ALTER TABLE [Order]
ADD ServedTime DATETIME2 NULL;

-- Cập nhật ServedTime cho các đơn hàng đã được phục vụ (Served = 1) 
-- Sử dụng UpdateTime nếu có, nếu không thì sử dụng CreateTime
UPDATE [Order] 
SET ServedTime = ISNULL(UpdateTime, CreateTime)
WHERE Served = 1 AND ServedTime IS NULL;

-- Kiểm tra kết quả
SELECT TOP 10 
    OrderCode,
    CreateTime,
    UpdateTime,
    Served,
    ServedTime,
    CASE 
        WHEN Served = 1 AND ServedTime IS NOT NULL 
        THEN DATEDIFF(MINUTE, CreateTime, ServedTime)
        ELSE NULL 
    END AS ServiceTimeMinutes
FROM [Order] 
WHERE Served = 1
ORDER BY CreateTime DESC;
