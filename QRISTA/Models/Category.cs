using System;
using System.ComponentModel.DataAnnotations;

namespace QRB.Models
{
    public class Category
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [MaxLength(255)]
        public string? CategoryCode { get; set; }
        [Required]
        [MaxLength(255)]
        public string? CategoryName { get; set; }
        [Required]
        public bool IsDelete { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
