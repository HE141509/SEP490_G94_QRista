using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Services
{
    public interface IMenuService
    {
        Task<(List<MenuItemDto> Items, int TotalCount)> GetMenuItemsAsync(int page = 1, int pageSize = 15, string? category = null);
        Task<List<string>> GetCategoriesAsync();
    }

    public class MenuService : IMenuService
    {
        private readonly QRBDbContext _context;

        public MenuService(QRBDbContext context)
        {
            _context = context;
        }

        public async Task<(List<MenuItemDto> Items, int TotalCount)> GetMenuItemsAsync(int page = 1, int pageSize = 15, string? category = null)
        {
            var query = _context.LoaiSanPhams
                .Include(l => l.SanPham)
                .Include(l => l.ChiNhanh)
                .Where(l => !l.IsDelete && !l.SanPham.IsDelete && !l.ChiNhanh.IsDelete);

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(l => l.SanPham.TenSanPham.ToLower().Contains(category.ToLower()));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new MenuItemDto
                {
                    Id = l.ID.ToString(),
                    Name = l.TenLoai,
                    Category = GetCategoryFromProductName(l.SanPham.TenSanPham),
                    Description = $"{l.SanPham.TenSanPham} - {l.TenLoai}",
                    Price = l.DonGia,
                    ImageUrl = "/images/cafe.png" // Default image
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var productNames = await _context.SanPhams
                .Where(s => !s.IsDelete)
                .Select(s => s.TenSanPham)
                .Distinct()
                .ToListAsync();

            var categories = new List<string> { "all" };
            
            foreach (var productName in productNames)
            {
                var category = GetCategoryFromProductName(productName);
                if (!categories.Contains(category))
                {
                    categories.Add(category);
                }
            }

            return categories;
        }

        private string GetCategoryFromProductName(string productName)
        {
            var lowerName = productName.ToLower();
            
            if (lowerName.Contains("cà phê") || lowerName.Contains("trà") || lowerName.Contains("nước"))
                return "drink";
            else if (lowerName.Contains("bánh") || lowerName.Contains("kem") || lowerName.Contains("chè"))
                return "dessert";
            else if (lowerName.Contains("cơm") || lowerName.Contains("phở") || lowerName.Contains("bún") || lowerName.Contains("mì"))
                return "main";
            else if (lowerName.Contains("combo"))
                return "combo";
            else
                return "other";
        }
    }

    public class MenuItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
