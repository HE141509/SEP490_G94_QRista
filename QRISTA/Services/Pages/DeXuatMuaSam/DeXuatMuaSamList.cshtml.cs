using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using Microsoft.EntityFrameworkCore;

namespace QRB.Pages.DeXuatMuaSam
{
    public class DeXuatMuaSamListModel : PageModel
    {
        private readonly QRBDbContext _context;
        public DeXuatMuaSamListModel(QRBDbContext context)
        {
            _context = context;
        }

        public class DeXuatMuaSamViewModel
        {
            public Guid ID { get; set; }
            public string MaDeXuat { get; set; } = string.Empty;
            public string TieuDe { get; set; } = string.Empty;
            public string NoiDungDeXuat { get; set; } = string.Empty;
            public string TenNguoiGui { get; set; } = string.Empty;
            public string TenChiNhanhGui { get; set; } = string.Empty;
            public string TenNguoiNhan { get; set; } = string.Empty;
            public string TenChiNhanhNhan { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime CreateTime { get; set; }
            public Guid IDNguoiGui { get; set; }
            public Guid IDChiNhanhGui { get; set; }
            public Guid IDNguoiNhan { get; set; }
            public Guid IDChiNhanhNhan { get; set; }
            public bool IsDelete { get; set; }
        }

        public class NguoiDungViewModel
        {
            public Guid ID { get; set; }
            public string TenHienThi { get; set; } = string.Empty;
        }

        public class ChiNhanhViewModel
        {
            public Guid ID { get; set; }
            public string TenChiNhanh { get; set; } = string.Empty;
        }

        public List<DeXuatMuaSamViewModel> DeXuatMuaSamList { get; set; } = new();
        public List<NguoiDungViewModel> NguoiDungList { get; set; } = new();  // Cho dropdown (đã lọc)
        public List<ChiNhanhViewModel> ChiNhanhList { get; set; } = new();     // Cho dropdown (đã lọc)
        public List<NguoiDungViewModel> AllNguoiDungList { get; set; } = new(); // Cho hiển thị bảng (không lọc)
        public List<ChiNhanhViewModel> AllChiNhanhList { get; set; } = new();   // Cho hiển thị bảng (không lọc)
        
        // User info
        public string CurrentUserBranchCode { get; set; } = string.Empty;
        public Guid CurrentUserId { get; set; } = Guid.Empty;
        public Guid CurrentUserBranchId { get; set; } = Guid.Empty;
        
        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                CurrentPage = pageNumber < 1 ? 1 : pageNumber;
                PageSize = pageSize < 5 ? 10 : (pageSize > 100 ? 100 : pageSize);

                // Lấy thông tin chi nhánh của user đang đăng nhập
                var userId = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid userGuid))
                {
                    CurrentUserId = userGuid;
                    
                    var currentUser = await _context.NguoiDungs
                        .Where(u => u.ID == userGuid && !u.IsDelete)
                        .Join(_context.ChiNhanhs, u => u.IDChiNhanh, c => c.ID, (u, c) => new { u, c })
                        .FirstOrDefaultAsync();
                    
                    if (currentUser != null)
                    {
                        CurrentUserBranchId = currentUser.c.ID;
                        
                        // Ưu tiên MaChiNhanh, nếu không có thì dùng 3 ký tự đầu của TenChiNhanh
                        CurrentUserBranchCode = !string.IsNullOrWhiteSpace(currentUser.c.MaChiNhanh) 
                            ? currentUser.c.MaChiNhanh 
                            : (currentUser.c.TenChiNhanh.Length >= 3 ? currentUser.c.TenChiNhanh.Substring(0, 3).ToUpper() : currentUser.c.TenChiNhanh.ToUpper());
                    }
                }

                // Đếm tổng số bản ghi (bao gồm cả đã xóa)
                TotalRecords = await _context.DeXuatMuaSams.CountAsync();

                // Lấy danh sách đề xuất mua sắm với phân trang (bao gồm cả đã xóa)
                DeXuatMuaSamList = await (from dx in _context.DeXuatMuaSams
                                         join nguoiGui in _context.NguoiDungs on dx.IDNguoiGui equals nguoiGui.ID into nguoiGuiGroup
                                         from nguoiGui in nguoiGuiGroup.DefaultIfEmpty()
                                         join chiNhanhGui in _context.ChiNhanhs on dx.IDChiNhanhGui equals chiNhanhGui.ID into chiNhanhGuiGroup
                                         from chiNhanhGui in chiNhanhGuiGroup.DefaultIfEmpty()
                                         join nguoiNhan in _context.NguoiDungs on dx.IDNguoiNhan equals nguoiNhan.ID into nguoiNhanGroup
                                         from nguoiNhan in nguoiNhanGroup.DefaultIfEmpty()
                                         join chiNhanhNhan in _context.ChiNhanhs on dx.IDChiNhanhNhan equals chiNhanhNhan.ID into chiNhanhNhanGroup
                                         from chiNhanhNhan in chiNhanhNhanGroup.DefaultIfEmpty()
                                         orderby dx.CreateTime descending
                                         select new DeXuatMuaSamViewModel
                                         {
                                             ID = dx.ID,
                                             MaDeXuat = dx.MaDeXuat,
                                             TieuDe = dx.TieuDe,
                                             NoiDungDeXuat = dx.NoiDungDeXuat,
                                             TenNguoiGui = nguoiGui != null ? nguoiGui.TenHienThi : "N/A",
                                             TenChiNhanhGui = chiNhanhGui != null ? chiNhanhGui.TenChiNhanh : "N/A",
                                             TenNguoiNhan = nguoiNhan != null ? nguoiNhan.TenHienThi : "N/A",
                                             TenChiNhanhNhan = chiNhanhNhan != null ? chiNhanhNhan.TenChiNhanh : "N/A",
                                             Status = dx.Status,
                                             CreateTime = dx.CreateTime,
                                             IDNguoiGui = dx.IDNguoiGui,
                                             IDChiNhanhGui = dx.IDChiNhanhGui,
                                             IDNguoiNhan = dx.IDNguoiNhan,
                                             IDChiNhanhNhan = dx.IDChiNhanhNhan,
                                             IsDelete = dx.IsDelete
                                         })
                                         .Skip((CurrentPage - 1) * PageSize)
                                         .Take(PageSize)
                                         .ToListAsync();

                // Lấy danh sách người dùng cho dropdown (loại trừ người cùng chi nhánh)
                NguoiDungList = await _context.NguoiDungs
                    .Where(nd => !nd.IsDelete && nd.IDChiNhanh != CurrentUserBranchId)
                    .Select(nd => new NguoiDungViewModel
                    {
                        ID = nd.ID,
                        TenHienThi = nd.TenHienThi
                    })
                    .OrderBy(nd => nd.TenHienThi)
                    .ToListAsync();

                // Lấy danh sách chi nhánh cho dropdown (loại trừ chi nhánh của người gửi)
                ChiNhanhList = await _context.ChiNhanhs
                    .Where(cn => !cn.IsDelete && cn.ID != CurrentUserBranchId)
                    .Select(cn => new ChiNhanhViewModel
                    {
                        ID = cn.ID,
                        TenChiNhanh = cn.TenChiNhanh
                    })
                    .OrderBy(cn => cn.TenChiNhanh)
                    .ToListAsync();

                // Lấy danh sách đầy đủ cho việc hiển thị trong dropdown người gửi và bảng
                AllNguoiDungList = await _context.NguoiDungs
                    .Where(nd => !nd.IsDelete)
                    .Select(nd => new NguoiDungViewModel
                    {
                        ID = nd.ID,
                        TenHienThi = nd.TenHienThi
                    })
                    .OrderBy(nd => nd.TenHienThi)
                    .ToListAsync();

                AllChiNhanhList = await _context.ChiNhanhs
                    .Where(cn => !cn.IsDelete)
                    .Select(cn => new ChiNhanhViewModel
                    {
                        ID = cn.ID,
                        TenChiNhanh = cn.TenChiNhanh
                    })
                    .OrderBy(cn => cn.TenChiNhanh)
                    .ToListAsync();
            }
            catch (Exception)
            {
                DeXuatMuaSamList = new List<DeXuatMuaSamViewModel>();
                NguoiDungList = new List<NguoiDungViewModel>();
                ChiNhanhList = new List<ChiNhanhViewModel>();
                AllNguoiDungList = new List<NguoiDungViewModel>();
                AllChiNhanhList = new List<ChiNhanhViewModel>();
                // Log error if needed
            }
        }

        // Handler method để lấy danh sách người dùng theo chi nhánh
        public async Task<JsonResult> OnGetGetUsersByBranchAsync(Guid branchId)
        {
            try
            {
                var users = await _context.NguoiDungs
                    .Where(u => u.IDChiNhanh == branchId && !u.IsDelete)
                    .Select(u => new
                    {
                        id = u.ID.ToString(),
                        tenHienThi = u.TenHienThi
                    })
                    .ToListAsync();

                return new JsonResult(users);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi lấy danh sách người dùng: " + ex.Message });
            }
        }

        // Handler method để lấy chi tiết đề xuất mua sắm
        public async Task<JsonResult> OnGetGetDeXuatDetailAsync(Guid id)
        {
            try
            {
                var deXuat = await (from dx in _context.DeXuatMuaSams
                                   join nguoiGui in _context.NguoiDungs on dx.IDNguoiGui equals nguoiGui.ID into nguoiGuiGroup
                                   from nguoiGui in nguoiGuiGroup.DefaultIfEmpty()
                                   join chiNhanhGui in _context.ChiNhanhs on dx.IDChiNhanhGui equals chiNhanhGui.ID into chiNhanhGuiGroup
                                   from chiNhanhGui in chiNhanhGuiGroup.DefaultIfEmpty()
                                   join nguoiNhan in _context.NguoiDungs on dx.IDNguoiNhan equals nguoiNhan.ID into nguoiNhanGroup
                                   from nguoiNhan in nguoiNhanGroup.DefaultIfEmpty()
                                   join chiNhanhNhan in _context.ChiNhanhs on dx.IDChiNhanhNhan equals chiNhanhNhan.ID into chiNhanhNhanGroup
                                   from chiNhanhNhan in chiNhanhNhanGroup.DefaultIfEmpty()
                                   where dx.ID == id && !dx.IsDelete
                                   select new
                                   {
                                       id = dx.ID.ToString(),
                                       maDeXuat = dx.MaDeXuat,
                                       tieuDe = dx.TieuDe,
                                       noiDungDeXuat = dx.NoiDungDeXuat,
                                       status = dx.Status,
                                       idNguoiGui = dx.IDNguoiGui.ToString(),
                                       tenNguoiGui = nguoiGui != null ? nguoiGui.TenHienThi : "N/A",
                                       idChiNhanhGui = dx.IDChiNhanhGui.ToString(),
                                       tenChiNhanhGui = chiNhanhGui != null ? chiNhanhGui.TenChiNhanh : "N/A",
                                       idNguoiNhan = dx.IDNguoiNhan.ToString(),
                                       tenNguoiNhan = nguoiNhan != null ? nguoiNhan.TenHienThi : "N/A",
                                       idChiNhanhNhan = dx.IDChiNhanhNhan.ToString(),
                                       tenChiNhanhNhan = chiNhanhNhan != null ? chiNhanhNhan.TenChiNhanh : "N/A"
                                   })
                                   .FirstOrDefaultAsync();

                if (deXuat == null)
                {
                    return new JsonResult(new { success = false, message = "Không tìm thấy đề xuất mua sắm" });
                }

                // Kiểm tra chỉ cho phép sửa khi ở trạng thái "Chờ duyệt"
                if (deXuat.status != "pending")
                {
                    return new JsonResult(new { success = false, message = "Chỉ có thể sửa đề xuất ở trạng thái 'Chờ duyệt'" });
                }

                return new JsonResult(new { success = true, data = deXuat });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Có lỗi khi lấy chi tiết đề xuất: " + ex.Message });
            }
        }
    }
}
