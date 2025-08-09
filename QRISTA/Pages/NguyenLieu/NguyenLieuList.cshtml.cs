using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QRB.Pages.NguyenLieu
{
    public class NguyenLieuListModel : PageModel
    {
        private readonly QRBDbContext _context;

        public NguyenLieuListModel(QRBDbContext context)
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

        public List<QRB.Models.NguyenLieu> NguyenLieuList { get; set; } = new List<QRB.Models.NguyenLieu>();

        public IActionResult OnGet(string? status)
        {
            // Kiểm tra đăng nhập - bắt buộc phải đăng nhập mới được truy cập
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                // Chưa đăng nhập, redirect về trang login
                return RedirectToPage("/Login");
            }
            if (!HasPermission("Full Raw Materials"))
            {
                return Redirect($"/AccessDenied?permission=Full Raw Materials&module=RawMaterials");
            }


            if (string.IsNullOrEmpty(status) || status == "active")
            {
                NguyenLieuList = _context.NguyenLieus.Where(nl => !nl.IsDelete).ToList();
            }
            else if (status == "inactive")
            {
                NguyenLieuList = _context.NguyenLieus.Where(nl => nl.IsDelete).ToList();
            }
            else
            {
                NguyenLieuList = _context.NguyenLieus.ToList();
            }

            return Page();
        }

        public IActionResult OnGetActiveNguyenLieu()
        {
            try
            {
                var activeNguyenLieu = _context.NguyenLieus
                    .Where(nl => !nl.IsDelete)
                    .Select(nl => new {
                        id = nl.ID.ToString(),
                        tenNguyenLieu = nl.TenNguyenLieu,
                        maNguyenLieu = nl.MaNguyenLieu,
                        donViTinh = nl.DonViTinh
                    })
                    .OrderBy(nl => nl.tenNguyenLieu)
                    .ToList();

                return new JsonResult(activeNguyenLieu);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }
    }
}
