using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRB.Models
{
    [Table("Roles")]
    public class AppRole
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
    }

    [Table("Permissions")]
    public class AppPermission
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
    }

    [Table("RolePermissions")]
    public class AppRolePermission
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string RoleId { get; set; } = string.Empty;
        
        [Required]
        public string PermissionId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual AppRole Role { get; set; } = null!;
        
        [ForeignKey("PermissionId")]
        public virtual AppPermission Permission { get; set; } = null!;
    }
}
