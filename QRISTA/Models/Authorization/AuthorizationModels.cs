using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRB.Models.Authorization
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
        
        public bool IsActive { get; set; } = true;
        
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
        public string Module { get; set; } = string.Empty;
        
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
        
        public DateTime GrantedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual AppRole Role { get; set; } = null!;
        
        [ForeignKey("PermissionId")]
        public virtual AppPermission Permission { get; set; } = null!;
    }

    // DTO Models for API requests
    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Staff";
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class CreateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class TogglePermissionRequest
    {
        public string RoleId { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public bool HasPermission { get; set; }
    }

    // View Models
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Email { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int PermissionCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PermissionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
    }

    // Batch update permissions request
    public class BatchUpdatePermissionsRequest
    {
        public List<PermissionChangeRequest> Changes { get; set; } = new List<PermissionChangeRequest>();
    }

    public class PermissionChangeRequest
    {
        public string RoleId { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
    }
}
