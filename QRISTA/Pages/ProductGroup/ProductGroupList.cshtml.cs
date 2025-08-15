using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QRB.Pages.ProductGroup
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    public class ProductGroupListModel : PageModel
    {
        private readonly QRBDbContext _context;
        public List<NhomSanPham> Groups { get; set; }

        public ProductGroupListModel(QRBDbContext context)
        {
            _context = context;
        }
        private bool HasPermission(string permissionName)
        {
            var permissionsJson = HttpContext.Session.GetString("UserPermissions");
            if (string.IsNullOrEmpty(permissionsJson))
            {
                return false;
            }
            try
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
                return permissions?.Contains(permissionName) ?? false;
            }
            catch
            {
                return false;
            }
        }
        public IActionResult OnGet()
        {
            // Kiểm tra session đăng nhập
            if (HttpContext.Session == null || string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToPage("/Login");
            }
             if (!HasPermission("Full Product Groups"))
            {
                return Redirect($"/AccessDenied?permission=Full Product Groups&module=ProductGroup");
            }

            Groups = _context.NhomSanPhams
                .OrderByDescending(x => x.CreateTime)
                .ToList();
            return Page();
        }
    }
}
