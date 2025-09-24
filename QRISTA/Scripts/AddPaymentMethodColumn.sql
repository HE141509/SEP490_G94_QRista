-- Thêm cột PaymentMethod vào bảng Order để lưu phương thức thanh toán
ALTER TABLE [Order]
ADD PaymentMethod NVARCHAR(100) NULL;

-- Cập nhật PaymentMethod cho các đơn hàng đã thanh toán (mặc định là "Tiền mặt")
UPDATE [Order] 
SET PaymentMethod = N'Tiền mặt'
WHERE PaymentStatus = 1 AND PaymentMethod IS NULL;

-- Kiểm tra kết quả
SELECT TOP 10 
    OrderCode,
    PaymentStatus,
    PaymentDate,
    PaymentMethod,
    Amount
FROM [Order] 
WHERE PaymentStatus = 1
ORDER BY CreateTime DESC;
