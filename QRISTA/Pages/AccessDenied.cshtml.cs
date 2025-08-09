using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QRB.Pages
{
    public class AccessDeniedModel : PageModel
    {
        public string Permission { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;

        public IActionResult OnGet(string? permission, string? module)
        {
            Permission = permission ?? "Unknown";
            Module = module ?? "Unknown";
            return Page();
        }
    }
}
