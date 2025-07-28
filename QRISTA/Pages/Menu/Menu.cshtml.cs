using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRB.Data;
using QRB.Models;
using System.Collections.Generic;
using System.Linq;

namespace QRB.Pages.Menu
{
    public class MenuModel : PageModel
    {
        private readonly QRBDbContext _context;
        public bool IsLoggedIn { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string ChiNhanhName { get; set; } = string.Empty;
        public List<NhomSanPham> ProductGroups { get; set; } = new();

        public MenuModel(QRBDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            IsLoggedIn = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
            if (IsLoggedIn)
            {
                DisplayName = HttpContext.Session.GetString("DisplayName") ?? "Người dùng";
                ChiNhanhName = HttpContext.Session.GetString("ChiNhanhName") ?? "Chi nhánh";
            }

            ProductGroups = _context.NhomSanPhams
                .Where(x => !x.IsDelete)
                .OrderBy(x => x.TenNhom)
                .ToList();
        }

        public JsonResult OnGetGetProductsByGroup(string maNhom)
        {
            var query = _context.SanPhams.Where(x => !x.IsDelete);
            if (!string.IsNullOrEmpty(maNhom) && maNhom != "all")
            {
                query = query.Where(x => x.NhomSanPham.MaNhom == maNhom);
            }

            var productTypes = _context.LoaiSanPhams
                .Where(x => !x.IsDelete && query.Select(p => p.ID).Contains(x.IDSanPham))
                .Select(x => new
                {
                    x.ID,
                    x.MaLoai,
                    x.TenLoai,
                    x.IDSanPham,
                    DonGiaRaw = Microsoft.EntityFrameworkCore.EF.Property<object>(x, "DonGia")
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.ID,
                    x.MaLoai,
                    x.TenLoai,
                    x.IDSanPham,
                    DonGia = TryParseDonGia(x.DonGiaRaw)
                })
                .ToList();

            var productIdsWithTypes = productTypes.Select(x => x.IDSanPham).Distinct().ToHashSet();
            var products = query.Where(x => productIdsWithTypes.Contains(x.ID)).Select(x => new
            {
                x.ID,
                x.MaSanPham,
                x.TenSanPham,
                x.HinhAnh,
                x.IdNhomSanPham,
                x.IDChiNhanh
            }).ToList();

            static decimal? TryParseDonGia(object donGiaObj)
            {
                if (donGiaObj == null) return null;
                if (donGiaObj is decimal d) return d;
                if (donGiaObj is double db) return (decimal)db;
                if (donGiaObj is float f) return (decimal)f;
                if (donGiaObj is int i) return i;
                if (donGiaObj is long l) return l;
                var str = donGiaObj.ToString();
                if (string.IsNullOrWhiteSpace(str)) return null;
                str = str.Replace(",", "").Replace(".00", "").Replace(" ", "").Trim();
                str = new string(str.Where(char.IsDigit).ToArray());
                if (string.IsNullOrWhiteSpace(str)) return null;
                if (decimal.TryParse(str, out var result)) return result;
                return null;
            }

            return new JsonResult(new { products, productTypes });
        }

        public JsonResult OnPostSaveOrderDetails([FromBody] SaveOrderRequest request)
        {
            try
            {
                HttpContext.Session.SetString("CartData", request.CartData ?? "");
                HttpContext.Session.SetString("qrb_cart_data", request.CartData ?? ""); // Thêm key cho Payment
                HttpContext.Session.SetString("PhoneNumber", request.PhoneNumber ?? "");
                HttpContext.Session.SetString("OrderTotalAmount", request.TotalAmount.ToString());
                HttpContext.Session.SetString("OrderDiscountAmount", request.DiscountAmount.ToString());
                
                return new JsonResult(new { success = true, message = "Đã lưu thông tin đơn hàng" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Lỗi khi lưu thông tin: {ex.Message}" });
            }
        }

        public JsonResult OnPostSaveCartData([FromBody] SaveCartRequest request)
        {
            try
            {
                Console.WriteLine($"SaveCartData called with: {request.CartData}");
                HttpContext.Session.SetString("qrb_cart_data", request.CartData ?? "");
                Console.WriteLine($"Saved to session. Session ID: {HttpContext.Session.Id}");
                return new JsonResult(new { success = true, message = "Đã lưu cart data" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveCartData: {ex.Message}");
                return new JsonResult(new { success = false, message = $"Lỗi khi lưu cart: {ex.Message}" });
            }
        }

        public JsonResult OnGetGetPhoneNumberFromSession()
        {
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
            return new JsonResult(new { phoneNumber });
        }

        public JsonResult OnGetGetCartDataFromSession()
        {
            var cartData = HttpContext.Session.GetString("qrb_cart_data");
            Console.WriteLine($"GetCartDataFromSession called. Session ID: {HttpContext.Session.Id}");
            Console.WriteLine($"Cart data from session: {cartData ?? "NULL"}");
            return new JsonResult(new { cartData });
        }
    }

    public class SaveOrderRequest
    {
        public string? CartData { get; set; }
        public string? PhoneNumber { get; set; }
        public int TotalAmount { get; set; }
        public int DiscountAmount { get; set; }
    }

    public class SaveCartRequest
    {
        public string? CartData { get; set; }
    }
}
