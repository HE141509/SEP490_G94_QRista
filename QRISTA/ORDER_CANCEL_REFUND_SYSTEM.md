# Hệ thống hủy đơn hàng và hoàn tiền - QRB

## Tổng quan tính năng mới

Hệ thống giờ đây hỗ trợ 2 quy trình xử lý đơn hàng theo yêu cầu:

### 🔄 **1. Hủy đơn hàng chưa trả hàng**
- **Áp dụng cho:** Đơn hàng chưa thanh toán hoặc đã thanh toán nhưng chưa trả hàng
- **Quyền hạn:** Nhân viên có thể thực hiện
- **Kết quả:** Đơn hàng chuyển trạng thái "Đã hủy"

### 💰 **2. Hoàn tiền đơn đã thanh toán** 
- **Áp dụng cho:** Đơn hàng đã thanh toán
- **Quyền hạn:** **CHỈ QUẢN LÝ** mới có quyền duyệt
- **Kết quả:** Đơn hàng chuyển trạng thái "Đã hoàn tiền"

## Database Schema Changes

### Các trường mới trong bảng `Order`:

```sql
[CancelReason] nvarchar(500) NULL,           -- Lý do hủy đơn
[CancelledByUserId] uniqueidentifier NULL,   -- ID người hủy đơn  
[CancelledDate] datetime2 NULL,              -- Ngày hủy đơn
[IsCancelled] bit NULL,                      -- Trạng thái đã hủy
[IsRefunded] bit NULL,                       -- Trạng thái đã hoàn tiền
[RefundAmount] nvarchar(255) NULL,           -- Số tiền hoàn
[RefundApprovedByUserId] uniqueidentifier NULL, -- ID người duyệt hoàn tiền
[RefundDate] datetime2 NULL,                 -- Ngày hoàn tiền
[RefundReason] nvarchar(500) NULL            -- Lý do hoàn tiền
```

## Logic nghiệp vụ

### 📋 **Quy trình hủy đơn:**

| Trạng thái đơn hàng | Điều kiện | Hành động cho phép | Người thực hiện |
|---------------------|-----------|-------------------|-----------------|
| Chưa thanh toán | - | ✅ Hủy đơn ngay lập tức | Nhân viên |
| Đã thanh toán + Chưa trả hàng | - | ⚠️ Yêu cầu duyệt hoàn tiền | Quản lý |
| Đã thanh toán + Đã trả hàng | - | ❌ Không cho phép hủy | - |
| Đã hủy/hoàn tiền | - | ❌ Không thể thao tác | - |

### 🔒 **Bảo mật và quyền hạn:**
- **Nhân viên:** Chỉ hủy được đơn chưa thanh toán
- **Quản lý:** Có thể duyệt hoàn tiền cho đơn đã thanh toán
- **Validation:** Kiểm tra trạng thái đơn, quyền người dùng, điều kiện nghiệp vụ

## Files Implementation

### 🆕 **Files mới:**
1. **`Pages/Order/CancelOrder.cshtml.cs`** - Backend logic hủy/hoàn tiền
2. **`Pages/Order/CancelOrder.cshtml`** - Razor page endpoint
3. **`UpdateOrderTable.sql`** - Script SQL cập nhật database

### 🔄 **Files cập nhật:**
1. **`Models/MenuItem.cs`** - Thêm các trường cancel/refund
2. **`Pages/Order/OrderList.cshtml`** - UI mới với nút hủy/hoàn tiền
3. **`Pages/Order/OrderList.cshtml.cs`** - Filter loại bỏ đơn đã hủy

## UI/UX Changes

### 🎨 **Giao diện mới:**

#### Cho đơn chưa thanh toán:
```
[Hủy] [Thanh toán]
```

#### Cho đơn đã thanh toán chưa trả hàng:
```
[Hoàn tiền] (màu cam - yêu cầu duyệt)
```

#### Cho đơn đã hoàn thành:
```
✅ Đã hoàn thành
```

#### Trạng thái đơn đã xử lý:
```
❌ Đã hủy
💰 Đã hoàn tiền
```

### 📱 **Modal dialogs:**
1. **Modal hủy đơn** - Nhập lý do hủy
2. **Modal hoàn tiền** - Nhập lý do + số tiền hoàn (chỉ quản lý)

## API Endpoints

### 🔗 **New endpoints:**

1. **`POST /Order/CancelOrder?handler=Cancel`**
   ```json
   {
     "orderId": "guid",
     "reason": "string",
     "requestType": "cancel"
   }
   ```

2. **`POST /Order/CancelOrder?handler=ApproveRefund`**
   ```json
   {
     "orderId": "guid", 
     "refundReason": "string",
     "refundAmount": "string"
   }
   ```

## Testing Scenarios

### ✅ **Test Cases:**

| Scenario | Expected Result | Status |
|----------|----------------|---------|
| Nhân viên hủy đơn chưa thanh toán | ✅ Hủy thành công | Pass |
| Nhân viên hủy đơn đã thanh toán | ❌ Hiển thị form hoàn tiền | Pass |
| Quản lý duyệt hoàn tiền | ✅ Hoàn tiền thành công | Pass |
| Nhân viên duyệt hoàn tiền | ❌ Từ chối quyền | Pass |
| Hủy đơn đã trả hàng | ❌ Từ chối với thông báo | Pass |
| Hủy đơn đã hủy trước đó | ❌ Từ chối với thông báo | Pass |

## Logging & Monitoring

### 📊 **Các event được log:**
- Hủy đơn hàng thành công/thất bại
- Yêu cầu hoàn tiền
- Duyệt hoàn tiền bởi quản lý
- Các attempt vi phạm quyền hạn

### 🎯 **Metrics theo dõi:**
- Số lượng đơn hàng bị hủy theo ngày
- Tỷ lệ hoàn tiền / tổng doanh thu
- Lý do hủy đơn phổ biến
- Thời gian xử lý hoàn tiền

## Security Features

### 🛡️ **Bảo mật đã triển khai:**
- ✅ Role-based authorization (Quản lý vs Nhân viên)
- ✅ Session validation
- ✅ Input validation & sanitization  
- ✅ Business rule enforcement
- ✅ Audit trail logging
- ✅ Anti-tampering checks

## Production Deployment

### 📋 **Checklist triển khai:**

1. **Database:**
   ```sql
   -- Chạy script UpdateOrderTable.sql
   ```

2. **Code Deploy:**
   - Deploy các files mới
   - Restart application

3. **Testing:**
   - Test với user role khác nhau
   - Verify permissions
   - Check logging

4. **Training:**
   - Hướng dẫn nhân viên quy trình mới
   - Hướng dẫn quản lý duyệt hoàn tiền

## ✅ **Kết quả đạt được:**

🎯 **Yêu cầu đã hoàn thành:**
- ✅ Chỉ được phép hủy đơn hàng chưa trả hàng
- ✅ Đơn đã thanh toán cần xác nhận quản lý để hoàn tiền
- ✅ Trạng thái hoàn tiền được quản lý chính xác
- ✅ UI/UX trực quan và dễ sử dụng
- ✅ Bảo mật và phân quyền chặt chẽ
- ✅ Audit trail đầy đủ

**Hệ thống giờ đây đã hỗ trợ đầy đủ quy trình hủy đơn/hoàn tiền theo yêu cầu nghiệp vụ!** 🚀
