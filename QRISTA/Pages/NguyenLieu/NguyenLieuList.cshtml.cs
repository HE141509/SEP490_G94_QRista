using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System.Collections.Generic;
using System.Linq;

namespace QRB.Pages.NguyenLieu
{
    public class NguyenLieuListModel : PageModel
    {
        private readonly QRBDbContext _context;

        public NguyenLieuListModel(QRBDbContext context)
        {
            _context = context;
        }

        public List<QRB.Models.NguyenLieu> NguyenLieuList { get; set; } = new List<QRB.Models.NguyenLieu>();

        public void OnGet(string? status)
        {
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
        }
    }
}
