using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRB.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DepartmentCode { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
        public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; } = new List<LoaiSanPham>();
        public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public virtual ICollection<KhoSanPham> KhoSanPhams { get; set; } = new List<KhoSanPham>();
        public virtual ICollection<DeXuatMuaSam> DeXuatMuaSams { get; set; } = new List<DeXuatMuaSam>();
    }

    // Compatibility class for UI - maps to Department table
    public class ChiNhanh
    {
        public Guid ID { get; set; }
        public string TenChiNhanh { get; set; } = string.Empty;
        public string MaChiNhanh { get; set; } = string.Empty;
        public bool IsDelete { get; set; } = false;
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? UpdateTime { get; set; }

        // Constructor for mapping from Department
        public ChiNhanh() { }

        public ChiNhanh(Department department)
        {
            ID = department.ID;
            TenChiNhanh = department.DepartmentName;
            MaChiNhanh = department.DepartmentCode;
            IsDelete = department.IsDelete;
            CreateTime = department.CreateTime;
            UpdateTime = department.UpdateTime;
        }

        // Convert to Department
        public Department ToDepartment()
        {
            return new Department
            {
                ID = this.ID,
                DepartmentName = this.TenChiNhanh,
                DepartmentCode = this.MaChiNhanh,
                IsDelete = this.IsDelete,
                CreateTime = this.CreateTime,
                UpdateTime = this.UpdateTime
            };
        }
    }

    [Table("Product")]
    public class SanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        [Column("ProductName")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("ProductCode")]
        public string MaSanPham { get; set; } = string.Empty;

        [Column("Picture")]
        public byte[]? HinhAnh { get; set; }

        public string? NoiDung { get; set; }

        [Required]
        public Guid IdCategory { get; set; } // matches new database column name
        [ForeignKey("IdCategory")]
        // navigation uses new Category type; FK column name in model is IdCategory
        public virtual Category Category { get; set; } = null!;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        [Required]
        [Column("IDDepartment")]
        public Guid IDChiNhanh { get; set; }

        // Navigation properties
        [ForeignKey("IDChiNhanh")]
        public virtual Department ChiNhanh { get; set; } = null!;
        public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; } = new List<LoaiSanPham>();
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }

    [Table("TypeProduct")]
    public class LoaiSanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [Column("IDProduct")]
        public Guid IDSanPham { get; set; }

        [Required]
        [StringLength(255)]
        [Column("TypeProductName")]
        public string TenLoai { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("TypeProductCode")]
        public string MaLoai { get; set; } = string.Empty;

        [Required]
        [Column("Price")]
        public string DonGia { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        [Required]
        [Column("IDDepartment")]
        public Guid IDChiNhanh { get; set; }

        // Navigation properties
        [ForeignKey("IDSanPham")]
        public virtual SanPham SanPham { get; set; } = null!;

        [ForeignKey("IDChiNhanh")]
        public virtual Department ChiNhanh { get; set; } = null!;

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }

    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string TenKhachHang { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SDT { get; set; } = string.Empty;

        public string? GiaTriDonHang { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Constructor from Customer for backward compatibility
        public KhachHang()
        {
        }

        public KhachHang(Customer customer)
        {
            ID = customer.ID;
            TenKhachHang = customer.CustomerName;
            SDT = customer.Phone;
            GiaTriDonHang = customer.GiaTriDonHang;
            IsDelete = customer.IsDelete;
            CreateTime = customer.CreateTime ?? DateTime.Now;
            UpdateTime = customer.UpdateTime;
        }

        // Convert to Customer
        public Customer ToCustomer()
        {
            return new Customer
            {
                ID = ID,
                CustomerName = TenKhachHang,
                Phone = SDT,
                GiaTriDonHang = GiaTriDonHang,
                IsDelete = IsDelete,
                CreateTime = CreateTime,
                UpdateTime = UpdateTime
            };
        }

        // Navigation properties
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public virtual ICollection<MaUuDai> MaUuDais { get; set; } = new List<MaUuDai>();
        public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }

    [Table("User")]
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        [Column("Account")]
        public string TenNguoiDung { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("Password")]
        public string MatKhau { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("UserName")]
        public string TenHienThi { get; set; } = string.Empty;

        [Required]
        [Column("IDDepartment")]
        public Guid IDChiNhanh { get; set; }

        // Thêm các trường cho hệ thống phân quyền
        [StringLength(50)]
        [Column("Role")]
        public string VaiTro { get; set; } = "Staff";

        [Column("Status")]
        public bool TrangThaiHoatDong { get; set; } = true;

        [StringLength(100)]
        public string? Email { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDChiNhanh")]
        public virtual Department ChiNhanh { get; set; } = null!;
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public virtual ICollection<QuyenSuDung> QuyenSuDungs { get; set; } = new List<QuyenSuDung>();
    }

    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Guid? IDKhachHang { get; set; }

        [Required]
        public Guid IDNhanVien { get; set; }

        [Required]
        public Guid IDChiNhanh { get; set; }

        [Required]
        [StringLength(255)]
        public string MaDonHang { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DonGia { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MaUuDai { get; set; }

        [StringLength(255)]
        public string? TienUuDai { get; set; }

        [Required]
        [StringLength(255)]
        public string TongTien { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        public bool TrangThaiThanhToan { get; set; } = false;

        public DateTime? NgayThanhToan { get; set; }

        public int? SoBan { get; set; }

        public bool? DaTraDon { get; set; }

        // Navigation properties
        [ForeignKey("IDKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("IDNhanVien")]
        public virtual NguoiDung NhanVien { get; set; } = null!;

        [ForeignKey("IDChiNhanh")]
        public virtual Department ChiNhanh { get; set; } = null!;

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }

    [Table("ChiTietDonHang")]
    public class ChiTietDonHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDDonHang { get; set; }

        [Required]
        public Guid IDSanPham { get; set; }

        [Required]
        public Guid IDLoaiSanPham { get; set; }

        public int SoLuong { get; set; }

        [Required]
        public string DonGia { get; set; } = string.Empty; // Đổi từ decimal sang string, thêm giá trị mặc định

        [Required]
        public string ThanhTien { get; set; } = string.Empty; // Đổi từ decimal sang string, thêm giá trị mặc định

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDDonHang")]
        public virtual DonHang DonHang { get; set; } = null!;

        [ForeignKey("IDSanPham")]
        public virtual SanPham SanPham { get; set; } = null!;

        [ForeignKey("IDLoaiSanPham")]
        public virtual LoaiSanPham LoaiSanPham { get; set; } = null!;
    }

    [Table("MaUuDai")]
    public class MaUuDai
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDKhachHang { get; set; }

        [Required]
        [StringLength(255)]
        public string MaGiamGia { get; set; } = string.Empty;

        [Required]
        public string TienGiam { get; set; } = string.Empty;

        public bool TrangThaiSuDung { get; set; } = false;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Constructor from Voucher for backward compatibility
        public MaUuDai()
        {
        }

        public MaUuDai(Voucher voucher)
        {
            ID = voucher.ID;
            IDKhachHang = voucher.IDCustomer;
            MaGiamGia = voucher.VoucherCode;
            TienGiam = voucher.Discount;
            TrangThaiSuDung = voucher.Status;
            IsDelete = voucher.IsDelete;
            CreateTime = voucher.CreateTime;
            UpdateTime = voucher.UpdateTime;
        }

        // Convert to Voucher
        public Voucher ToVoucher()
        {
            return new Voucher
            {
                ID = ID,
                IDCustomer = IDKhachHang,
                VoucherCode = MaGiamGia,
                Discount = TienGiam,
                Status = TrangThaiSuDung,
                IsDelete = IsDelete,
                CreateTime = CreateTime,
                UpdateTime = UpdateTime
            };
        }

        [ForeignKey("IDCustomer")]
        public virtual CustomerInfo CustomerInfo { get; set; } = null!;
    }

    [Table("Voucher")]
    public class Voucher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDCustomer { get; set; }

        [Required]
        [StringLength(255)]
        public string VoucherCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Discount { get; set; } = string.Empty;

        public bool Status { get; set; } = false;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        [ForeignKey("IDCustomer")]
        public virtual Customer Customer { get; set; } = null!;
    }

    [Table("NguyenLieu")]
    public class NguyenLieu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string TenNguyenLieu { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MaNguyenLieu { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DonViTinh { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Constructor from Ingredient for backward compatibility
        public NguyenLieu()
        {
        }

        public NguyenLieu(Ingredient ingredient)
        {
            ID = ingredient.ID;
            TenNguyenLieu = ingredient.IngredientName;
            MaNguyenLieu = ingredient.IngredientCode;
            DonViTinh = ingredient.UnitOfMeasure;
            IsDelete = ingredient.IsDeleted;
            CreateTime = ingredient.CreateTime;
            UpdateTime = ingredient.UpdateTime;
        }

        // Convert to Ingredient
        public Ingredient ToIngredient()
        {
            return new Ingredient
            {
                ID = ID,
                IngredientName = TenNguyenLieu,
                IngredientCode = MaNguyenLieu,
                UnitOfMeasure = DonViTinh,
                IsDeleted = IsDelete,
                CreateTime = CreateTime,
                UpdateTime = UpdateTime
            };
        }

        // Navigation properties
        public virtual ICollection<KhoSanPham> KhoSanPhams { get; set; } = new List<KhoSanPham>();
    }

    [Table("Ingredient")]
    public class Ingredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string IngredientName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string IngredientCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        public virtual ICollection<KhoSanPham> KhoSanPhams { get; set; } = new List<KhoSanPham>();
    }

    [Table("KhoSanPham")]
    public class KhoSanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDNguyenLieu { get; set; }

        [Required]
        [StringLength(50)]
        public string SoLuongConLai { get; set; } = string.Empty;

        [Required]
        public Guid IDChiNhanh { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDNguyenLieu")]
        public virtual Ingredient Ingredient { get; set; } = null!;

        [ForeignKey("IDChiNhanh")]
        public virtual Department ChiNhanh { get; set; } = null!;
    }

    [Table("QuyenSuDung")]
    public class QuyenSuDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string MaQuyen { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TenQuyen { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
    }

    [Table("Request")]
    public class DeXuatMuaSam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [Column("IDSender")]
        public Guid IDNguoiGui { get; set; }

        [Required]
        [Column("IDSenDDepartment")]
        public Guid IDChiNhanhGui { get; set; }

        [Required]
        [Column("IDReceiver")]
        public Guid IDNguoiNhan { get; set; }

        [Required]
        [Column("IDReceiveDepartment")]
        public Guid IDChiNhanhNhan { get; set; }

        [Required]
        [StringLength(255)]
        [Column("RequestCode")]
        public string MaDeXuat { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("Title")]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        [Column("Description")]
        public string NoiDungDeXuat { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? AcceptTime { get; set; }

        public DateTime? RejectTime { get; set; }

        public DateTime? ReceiveTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool IsDelete { get; set; } = false;

        [StringLength(500)]
        [Column("RejectTitle")]
        public string? NoiDungTuChoi { get; set; }

        // Navigation properties (nếu cần)
        public virtual NguyenLieu? NguoiGui { get; set; }
        public virtual Department? ChiNhanhGui { get; set; }
        public virtual NguyenLieu? NguoiNhan { get; set; }
        public virtual Department? ChiNhanhNhan { get; set; }
        public virtual ICollection<ChiTietDonDeXuat> ChiTietDonDeXuats { get; set; } = new List<ChiTietDonDeXuat>();
    }

    [Table("RequestDetail")]
    public class ChiTietDonDeXuat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [Column("IDRequest")]
        public Guid IDDeXuatMuaSam { get; set; }

        [Required]
        [Column("IDNguyenLieu")]
        public Guid IDNguyenLieu { get; set; }

        [Column("Quantity")]
        public int SoLuong { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDDeXuatMuaSam")]
        public virtual DeXuatMuaSam DeXuatMuaSam { get; set; } = null!;
    }

    // New Order table model following the new schema
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Guid? IDCustomer { get; set; }

        [Required]
        public Guid IDEmployee { get; set; }

        [Required]
        public Guid IDDepartment { get; set; }

        [Required]
        [StringLength(255)]
        public string OrderCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Price { get; set; } = string.Empty;

        [StringLength(255)]
        public string? VoucherCode { get; set; }

        [StringLength(255)]
        public string? VoucherPrice { get; set; }

        [Required]
        [StringLength(255)]
        public string Amount { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        public bool? PaymentStatus { get; set; } = false;

        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        public string? PaymentMethod { get; set; } // "Tiền mặt", "VnPay", "Chuyển khoản", etc.

        public int? Table { get; set; }

        public bool? Served { get; set; } = false;

        public DateTime? ServedTime { get; set; }

        // Cancel/Refund fields
        public bool? IsCancelled { get; set; } = false;

        public DateTime? CancelledDate { get; set; }

        [StringLength(500)]
        public string? CancelReason { get; set; }

        public Guid? CancelledByUserId { get; set; }

        // Refund fields (for paid orders)
        public bool? IsRefunded { get; set; } = false;

        public DateTime? RefundDate { get; set; }

        [StringLength(500)]
        public string? RefundReason { get; set; }

        public Guid? RefundApprovedByUserId { get; set; }

        [StringLength(255)]
        public string? RefundAmount { get; set; }

        // Navigation properties
        [ForeignKey("IDCustomer")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("IDEmployee")]
        public virtual NguoiDung Employee { get; set; } = null!;

        [ForeignKey("IDDepartment")]
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    // New OrderDetail table model
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDOrder { get; set; }

        [Required]
        public Guid IDProduct { get; set; }

        [Required]
        [Column("IDTypeProduct")]
        public Guid IDProductType { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(255)]
        public string Price { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("Amount")]
        public string Total { get; set; } = string.Empty;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDOrder")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("IDProduct")]
        public virtual SanPham Product { get; set; } = null!;

        [ForeignKey("IDProductType")]
        public virtual LoaiSanPham ProductType { get; set; } = null!;
    }
}
