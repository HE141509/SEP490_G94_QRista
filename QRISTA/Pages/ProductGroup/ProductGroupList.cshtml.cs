using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Models;
using QRB.Data;
using System.Collections.Generic;
using System.Linq;

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

    public IActionResult OnGet()
    {
        // Kiểm tra session đăng nhập
        if (HttpContext.Session == null || string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
        {
            return RedirectToPage("/Login");
        }
        Groups = _context.NhomSanPhams
            .OrderByDescending(x => x.CreateTime)
            .ToList();
        return Page();
    }
}
}
