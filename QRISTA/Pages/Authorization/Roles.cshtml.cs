using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models.Authorization;

namespace QRB.Pages.Authorization
{
    public class RolesModel : PageModel
    {
        private readonly QRBDbContext _context;

        public RolesModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<RoleWithUserCount> Roles { get; set; } = new List<RoleWithUserCount>();

        public async Task OnGetAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            
            Roles = roles.Select(r => new RoleWithUserCount
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                UserCount = _context.NguoiDungs.Count(u => u.VaiTro == r.Name && !u.IsDelete)
            }).ToList();
        }
    }

    public class RoleWithUserCount
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int UserCount { get; set; }
    }
}
