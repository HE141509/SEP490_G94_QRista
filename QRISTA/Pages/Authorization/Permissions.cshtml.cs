using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models.Authorization;

namespace QRB.Pages.Authorization
{
    public class PermissionsModel : PageModel
    {
        private readonly QRBDbContext _context;

        public PermissionsModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<AppRole> Roles { get; set; } = new List<AppRole>();
        public List<AppPermission> Permissions { get; set; } = new List<AppPermission>();
        public List<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
        public Dictionary<string, List<AppPermission>> PermissionGroups { get; set; } = new Dictionary<string, List<AppPermission>>();

        public async Task OnGetAsync()
        {
            // Load roles
            Roles = await _context.Roles.ToListAsync();

            // Load permissions
            Permissions = await _context.Permissions.OrderBy(p => p.Module).ThenBy(p => p.Name).ToListAsync();

            // Load role permissions
            RolePermissions = await _context.RolePermissions.ToListAsync();

            // Group permissions by module
            PermissionGroups = Permissions.GroupBy(p => p.Module)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
