using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRB.Models
{
    [Table("ChiNhanh")]
    public class ChiNhanh
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string TenChiNhanh { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MaChiNhanh { get; set; } = string.Empty;

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

    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string TenSanPham { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MaSanPham { get; set; } = string.Empty;

        public byte[]? HinhAnh { get; set; }

        [Required]
        public Guid IdNhomSanPham { get; set; } // map đúng tên cột DB
        [ForeignKey("IdNhomSanPham")]
        public virtual NhomSanPham NhomSanPham { get; set; } = null!;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        [Required]
        public Guid IDChiNhanh { get; set; }

        // Navigation properties
        [ForeignKey("IDChiNhanh")]
        public virtual ChiNhanh ChiNhanh { get; set; } = null!;
        public virtual ICollection<LoaiSanPham> LoaiSanPhams { get; set; } = new List<LoaiSanPham>();
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public virtual ICollection<KhoSanPham> KhoSanPhams { get; set; } = new List<KhoSanPham>();
    }

    [Table("LoaiSanPham")]
    public class LoaiSanPham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDSanPham { get; set; }

        [Required]
        [StringLength(255)]
        public string TenLoai { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MaLoai { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DonGia { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        [Required]
        public Guid IDChiNhanh { get; set; }

        // Navigation properties
        [ForeignKey("IDSanPham")]
        public virtual SanPham SanPham { get; set; } = null!;

        [ForeignKey("IDChiNhanh")]
        public virtual ChiNhanh ChiNhanh { get; set; } = null!;

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

        [Column(TypeName = "decimal(15,2)")]
        public decimal? GiaTriDonHang { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public virtual ICollection<MaUuDai> MaUuDais { get; set; } = new List<MaUuDai>();
    }

    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [StringLength(255)]
        public string TenNguoiDung { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TenHienThi { get; set; } = string.Empty;

        [Required]
        public Guid IDChiNhanh { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDChiNhanh")]
        public virtual ChiNhanh ChiNhanh { get; set; } = null!;
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public virtual ICollection<QuyenSuDung> QuyenSuDungs { get; set; } = new List<QuyenSuDung>();
    }

    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDKhachHang { get; set; }

        [Required]
        public Guid IDNhanVien { get; set; }

        [Required]
        public Guid IDChiNhanh { get; set; }

        [Required]
        [StringLength(255)]
        public string MaDonHang { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal DonGia { get; set; }

        [StringLength(255)]
        public string? MaUuDai { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? TienUuDai { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal TongTien { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDKhachHang")]
        public virtual KhachHang KhachHang { get; set; } = null!;

        [ForeignKey("IDNhanVien")]
        public virtual NguoiDung NhanVien { get; set; } = null!;

        [ForeignKey("IDChiNhanh")]
        public virtual ChiNhanh ChiNhanh { get; set; } = null!;

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
        [Column(TypeName = "decimal(10,2)")]
        public decimal DonGia { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal ThanhTien { get; set; }

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
        [Column(TypeName = "decimal(15,2)")]
        public decimal TienGiam { get; set; }

        public bool TrangThaiSuDung { get; set; } = false;

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDKhachHang")]
        public virtual KhachHang KhachHang { get; set; } = null!;
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

        // Navigation properties
        public virtual ICollection<KhoSanPham> KhoSanPhams { get; set; } = new List<KhoSanPham>();
        public virtual ICollection<ChiTietDonDeXuat> ChiTietDonDeXuats { get; set; } = new List<ChiTietDonDeXuat>();
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
        public Guid IDSanPham { get; set; }

        public int SoLuongConLai { get; set; }

        [Required]
        public Guid IDChiNhanh { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDNguyenLieu")]
        public virtual NguyenLieu NguyenLieu { get; set; } = null!;

        [ForeignKey("IDSanPham")]
        public virtual SanPham SanPham { get; set; } = null!;

        [ForeignKey("IDChiNhanh")]
        public virtual ChiNhanh ChiNhanh { get; set; } = null!;
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

    [Table("DeXuatMuaSam")]
    public class DeXuatMuaSam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDNguoiGui { get; set; }

        [Required]
        public Guid IDChiNhanhGui { get; set; }

        [Required]
        public Guid IDNguoiNhan { get; set; }

        [Required]
        public Guid IDChiNhanhNhan { get; set; }

        [Required]
        [StringLength(255)]
        public string MaDeXuat { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        public string NoiDungDeXuat { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "PENDING";

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? AcceptTime { get; set; }

        public DateTime? ReceiveTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDNguoiGui")]
        public virtual NguoiDung NguoiGui { get; set; } = null!;

        [ForeignKey("IDChiNhanhGui")]
        public virtual ChiNhanh ChiNhanhGui { get; set; } = null!;

        [ForeignKey("IDNguoiNhan")]
        public virtual NguoiDung NguoiNhan { get; set; } = null!;

        [ForeignKey("IDChiNhanhNhan")]
        public virtual ChiNhanh ChiNhanhNhan { get; set; } = null!;

        public virtual ICollection<ChiTietDonDeXuat> ChiTietDonDeXuats { get; set; } = new List<ChiTietDonDeXuat>();
    }

    [Table("ChiTietDonDeXuat")]
    public class ChiTietDonDeXuat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public Guid IDDeXuatMuaSam { get; set; }

        [Required]
        public Guid IDNguyenLieu { get; set; }

        public int SoLuong { get; set; }

        public bool IsDelete { get; set; } = false;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime? UpdateTime { get; set; }

        // Navigation properties
        [ForeignKey("IDDeXuatMuaSam")]
        public virtual DeXuatMuaSam DeXuatMuaSam { get; set; } = null!;

        [ForeignKey("IDNguyenLieu")]
        public virtual NguyenLieu NguyenLieu { get; set; } = null!;
    }
}
