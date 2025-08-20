using System;

namespace QRB.Models
{
    public class CustomerInfo
    {
        public int STT { get; set; }
        public Guid Id { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string? GiaTriDonHang { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
