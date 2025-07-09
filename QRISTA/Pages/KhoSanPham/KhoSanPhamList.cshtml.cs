using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System.Collections.Generic;
using System.Linq;

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
        public void OnGet(string? status = "active")
        {
            Status = status ?? "active";
            
            // Lấy danh sách nguyên liệu
            NguyenLieuList = _context.NguyenLieus
                .Where(n => !n.IsDelete)
                .Select(n => new NguyenLieuViewModel
                {
                    ID = n.ID,
                    TenNguyenLieu = n.TenNguyenLieu
                })
                .OrderBy(n => n.TenNguyenLieu)
                .ToList();
                
            // Lấy danh sách chi nhánh
            ChiNhanhList = _context.ChiNhanhs
                .Where(c => !c.IsDelete)
                .Select(c => new ChiNhanhViewModel
                {
                    ID = c.ID,
                    TenChiNhanh = c.TenChiNhanh
                })
                .OrderBy(c => c.TenChiNhanh)
                .ToList();
            
            var query = _context.KhoSanPhams
                .Join(_context.NguyenLieus, k => k.IDNguyenLieu, n => n.ID, (k, n) => new { k, n })
                .Join(_context.ChiNhanhs, kn => kn.k.IDChiNhanh, c => c.ID, (kn, c) => new { kn.k, kn.n, c });

            if (Status == "active")
                query = query.Where(x => !x.k.IsDelete);
            else if (Status == "inactive")
                query = query.Where(x => x.k.IsDelete);

            KhoSanPhamList = query
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
                .ToList();
        }
    }
}
