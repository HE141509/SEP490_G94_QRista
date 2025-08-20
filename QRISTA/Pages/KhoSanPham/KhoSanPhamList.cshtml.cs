using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
=======
using System.Text.Json;
>>>>>>> origin/phuong2

namespace QRB.Pages.KhoSanPham
{
    public class KhoSanPhamListModel : PageModel
    {
        private readonly QRBDbContext _context;
        public KhoSanPhamListModel(QRBDbContext context)
        {
            _context = context;
        }

        public class KhoSanPhamViewModel
        {
            public Guid ID { get; set; }
            public string TenNguyenLieu { get; set; } = string.Empty;
            public string SoLuongConLai { get; set; } = string.Empty;
            public string TenChiNhanh { get; set; } = string.Empty;
            public DateTime CreateTime { get; set; }
            public bool IsDelete { get; set; }
            public Guid IDNguyenLieu { get; set; }
            public Guid IDChiNhanh { get; set; }
        }

        public List<KhoSanPhamViewModel> KhoSanPhamList { get; set; } = new();
        public List<NguyenLieuViewModel> NguyenLieuList { get; set; } = new();
        public List<ChiNhanhViewModel> ChiNhanhList { get; set; } = new();
        public string Status { get; set; } = "active";
        
        // Thông tin chi nhánh của user hiện tại
        public Guid CurrentUserBranchId { get; set; } = Guid.Empty;
        
        public class NguyenLieuViewModel
        {
            public Guid ID { get; set; }
            public string TenNguyenLieu { get; set; } = string.Empty;
        }
        
        public class ChiNhanhViewModel
        {
            public Guid ID { get; set; }
            public string TenChiNhanh { get; set; } = string.Empty;
        }
<<<<<<< HEAD
=======
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
>>>>>>> origin/phuong2
        public async Task<IActionResult> OnGetAsync(string? status = "active")
        {
            // Kiểm tra đăng nhập - bắt buộc phải đăng nhập mới được truy cập
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                // Chưa đăng nhập, redirect về trang login
                return RedirectToPage("/Login");
            }
<<<<<<< HEAD

            Status = status ?? "active";
            
=======
            if (!HasPermission("Full Inventory"))
            {
                return Redirect($"/AccessDenied?permission=Full Inventory&module=Inventory");
            }

            Status = status ?? "active";

>>>>>>> origin/phuong2
            // Lấy thông tin chi nhánh của user đang đăng nhập
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out userGuid))
            {
                var currentUser = await _context.NguoiDungs
                    .Where(u => u.ID == userGuid && !u.IsDelete)
                    .FirstOrDefaultAsync();
<<<<<<< HEAD
                
=======

>>>>>>> origin/phuong2
                if (currentUser != null)
                {
                    CurrentUserBranchId = currentUser.IDChiNhanh;
                }
            }
<<<<<<< HEAD
            
=======

>>>>>>> origin/phuong2
            // Lấy danh sách nguyên liệu
            NguyenLieuList = await _context.NguyenLieus
                .Where(n => !n.IsDelete)
                .Select(n => new NguyenLieuViewModel
                {
                    ID = n.ID,
                    TenNguyenLieu = n.TenNguyenLieu
                })
                .OrderBy(n => n.TenNguyenLieu)
                .ToListAsync();
<<<<<<< HEAD
                
=======

>>>>>>> origin/phuong2
            // Lấy danh sách chi nhánh (chỉ chi nhánh của user hiện tại)
            ChiNhanhList = await _context.ChiNhanhs
                .Where(c => !c.IsDelete && c.ID == CurrentUserBranchId)
                .Select(c => new ChiNhanhViewModel
                {
                    ID = c.ID,
                    TenChiNhanh = c.TenChiNhanh
                })
                .OrderBy(c => c.TenChiNhanh)
                .ToListAsync();
<<<<<<< HEAD
            
=======

>>>>>>> origin/phuong2
            // Lấy danh sách kho sản phẩm chỉ thuộc chi nhánh của user hiện tại
            var query = _context.KhoSanPhams
                .Where(k => k.IDChiNhanh == CurrentUserBranchId) // Chỉ lấy kho sản phẩm của chi nhánh hiện tại
                .Join(_context.NguyenLieus, k => k.IDNguyenLieu, n => n.ID, (k, n) => new { k, n })
                .Join(_context.ChiNhanhs, kn => kn.k.IDChiNhanh, c => c.ID, (kn, c) => new { kn.k, kn.n, c });

            if (Status == "active")
                query = query.Where(x => !x.k.IsDelete);
            else if (Status == "inactive")
                query = query.Where(x => x.k.IsDelete);

            KhoSanPhamList = await query
                .OrderByDescending(x => x.k.CreateTime)
                .Select(x => new KhoSanPhamViewModel
                {
                    ID = x.k.ID,
                    TenNguyenLieu = x.n.TenNguyenLieu,
                    SoLuongConLai = x.k.SoLuongConLai,
                    TenChiNhanh = x.c.TenChiNhanh,
                    CreateTime = x.k.CreateTime,
                    IsDelete = x.k.IsDelete,
                    IDNguyenLieu = x.k.IDNguyenLieu,
                    IDChiNhanh = x.k.IDChiNhanh
                })
                .ToListAsync();

            return Page();
        }
    }
}
