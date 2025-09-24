using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRB.Models
{
    [Table("UserBranches")]
    public class UserBranch
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid BranchId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual NguoiDung? NguoiDung { get; set; }
        public virtual ChiNhanh? ChiNhanh { get; set; }
    }
}
