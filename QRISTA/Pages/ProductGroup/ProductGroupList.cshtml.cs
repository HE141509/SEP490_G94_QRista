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
        public List<Category> Groups { get; set; } = new();

        public ProductGroupListModel(QRBDbContext context)
        {
            _context = context;
        }

        // ===== Giữ nguyên code HasPermission =====
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

        // ===== Giữ nguyên OnGet =====
        public IActionResult OnGet()
        {
            if (HttpContext.Session == null || string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToPage("/Login");
            }
            if (!HasPermission("Full Product Groups"))
            {
                return Redirect($"/AccessDenied?permission=Full Product Groups&module=ProductGroup");
            }

            Groups = _context.Categories
                .OrderByDescending(x => x.CreateTime)
                .ToList();
            return Page();
        }

        // ===== Mình thêm mới ở đây: Handler cho AJAX AddGroup =====
        [ValidateAntiForgeryToken]
        public IActionResult OnPostAddGroup([FromBody] Category model)
        {
            if (model == null)
            {
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            try
            {
                // Check trùng CategoryCode
                var exists = _context.Categories.Any(c => c.CategoryCode == model.CategoryCode);
                if (exists)
                {
                    return new JsonResult(new { success = false, message = "Mã nhóm đã tồn tại!" });
                }

                model.ID = Guid.NewGuid();
                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;

                _context.Categories.Add(model);
                _context.SaveChanges();

                return new JsonResult(new { success = true, message = "Thêm nhóm thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}


// using Microsoft.AspNetCore.Mvc.RazorPages;
// using QRB.Models;
// using QRB.Data;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.Json;

// namespace QRB.Pages.ProductGroup
// {
//     using Microsoft.AspNetCore.Http;
//     using Microsoft.AspNetCore.Mvc;
//     using System;
//     public class ProductGroupListModel : PageModel
//     {
//         private readonly QRBDbContext _context;
//     public List<Category> Groups { get; set; } = new();

//         public ProductGroupListModel(QRBDbContext context)
//         {
//             _context = context;
//         }
//         private bool HasPermission(string permissionName)
//         {
//             var permissionsJson = HttpContext.Session.GetString("UserPermissions");
//             if (string.IsNullOrEmpty(permissionsJson))
//             {
//                 return false;
//             }
//             try
//             {
//                 var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
//                 return permissions?.Contains(permissionName) ?? false;
//             }
//             catch
//             {
//                 return false;
//             }
//         }
//         public IActionResult OnGet()
//         {
//             // Kiểm tra session đăng nhập
//             if (HttpContext.Session == null || string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
//             {
//                 return RedirectToPage("/Login");
//             }
//              if (!HasPermission("Full Product Groups"))
//             {
//                 return Redirect($"/AccessDenied?permission=Full Product Groups&module=ProductGroup");
//             }

//             Groups = _context.Categories
//                 .OrderByDescending(x => x.CreateTime)
//                 .ToList();
//             return Page();
//         }
//     }
// }


