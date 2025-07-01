using System;
using System.ComponentModel.DataAnnotations;

namespace QRB.Models
{
    public class NhomSanPham
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [MaxLength(255)]
        public string? MaNhom { get; set; }
        [Required]
        [MaxLength(255)]
        public string? TenNhom { get; set; }
        [Required]
        public bool IsDelete { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
