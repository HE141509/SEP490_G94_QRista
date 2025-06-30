using Microsoft.EntityFrameworkCore;
using QRB.Models;

namespace QRB.Data
{
    public class QRBDbContext : DbContext
    {
        public QRBDbContext(DbContextOptions<QRBDbContext> options) : base(options)
        {
        }

        // DbSets cho tất cả các bảng
        public DbSet<ChiNhanh> ChiNhanhs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<MaUuDai> MaUuDais { get; set; }
        public DbSet<NguyenLieu> NguyenLieus { get; set; }
        public DbSet<KhoSanPham> KhoSanPhams { get; set; }
        public DbSet<QuyenSuDung> QuyenSuDungs { get; set; }
        public DbSet<DeXuatMuaSam> DeXuatMuaSams { get; set; }
        public DbSet<ChiTietDonDeXuat> ChiTietDonDeXuats { get; set; }
        public DbSet<NhomSanPham> NhomSanPhams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NhomSanPham: mapping đúng tên bảng vật lý trong DB
            modelBuilder.Entity<NhomSanPham>().ToTable("NhomSanPham");
            base.OnModelCreating(modelBuilder);

            // Cấu hình các quan hệ và ràng buộc
            
            // ChiNhanh
            modelBuilder.Entity<ChiNhanh>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaChiNhanh).IsUnique();
            });

            // SanPham
            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaSanPham).IsUnique();
                entity.HasOne(d => d.ChiNhanh)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.IDChiNhanh);
            });

            // LoaiSanPham
            modelBuilder.Entity<LoaiSanPham>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaLoai).IsUnique();
                entity.HasOne(d => d.SanPham)
                    .WithMany(p => p.LoaiSanPhams)
                    .HasForeignKey(d => d.IDSanPham);
                entity.HasOne(d => d.ChiNhanh)
                    .WithMany(p => p.LoaiSanPhams)
                    .HasForeignKey(d => d.IDChiNhanh);
            });

            // KhachHang
            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.SDT).IsUnique();
            });

            // NguoiDung
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.TenNguoiDung).IsUnique();
                entity.HasOne(d => d.ChiNhanh)
                    .WithMany(p => p.NguoiDungs)
                    .HasForeignKey(d => d.IDChiNhanh);
            });

            // DonHang
            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaDonHang).IsUnique();
                entity.HasOne(d => d.KhachHang)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.IDKhachHang);
                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.IDNhanVien);
                entity.HasOne(d => d.ChiNhanh)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.IDChiNhanh);
            });

            // ChiTietDonHang
            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasOne(d => d.DonHang)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.IDDonHang);
                entity.HasOne(d => d.SanPham)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.IDSanPham);
                entity.HasOne(d => d.LoaiSanPham)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.IDLoaiSanPham);
            });

            // MaUuDai
            modelBuilder.Entity<MaUuDai>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaGiamGia).IsUnique();
                entity.HasOne(d => d.KhachHang)
                    .WithMany(p => p.MaUuDais)
                    .HasForeignKey(d => d.IDKhachHang);
            });

            // NguyenLieu
            modelBuilder.Entity<NguyenLieu>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaNguyenLieu).IsUnique();
            });

            // KhoSanPham
            modelBuilder.Entity<KhoSanPham>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasOne(d => d.NguyenLieu)
                    .WithMany(p => p.KhoSanPhams)
                    .HasForeignKey(d => d.IDNguyenLieu);
                entity.HasOne(d => d.SanPham)
                    .WithMany(p => p.KhoSanPhams)
                    .HasForeignKey(d => d.IDSanPham);
                entity.HasOne(d => d.ChiNhanh)
                    .WithMany(p => p.KhoSanPhams)
                    .HasForeignKey(d => d.IDChiNhanh);
            });

            // QuyenSuDung
            modelBuilder.Entity<QuyenSuDung>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaQuyen).IsUnique();
            });

            // DeXuatMuaSam
            modelBuilder.Entity<DeXuatMuaSam>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaDeXuat).IsUnique();
                entity.HasOne(d => d.NguoiGui)
                    .WithMany()
                    .HasForeignKey(d => d.IDNguoiGui)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.ChiNhanhGui)
                    .WithMany()
                    .HasForeignKey(d => d.IDChiNhanhGui)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.NguoiNhan)
                    .WithMany()
                    .HasForeignKey(d => d.IDNguoiNhan)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.ChiNhanhNhan)
                    .WithMany(p => p.DeXuatMuaSams)
                    .HasForeignKey(d => d.IDChiNhanhNhan)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ChiTietDonDeXuat
            modelBuilder.Entity<ChiTietDonDeXuat>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasOne(d => d.DeXuatMuaSam)
                    .WithMany(p => p.ChiTietDonDeXuats)
                    .HasForeignKey(d => d.IDDeXuatMuaSam);
                entity.HasOne(d => d.NguyenLieu)
                    .WithMany(p => p.ChiTietDonDeXuats)
                    .HasForeignKey(d => d.IDNguyenLieu);
            });

            // Seed data mẫu
            SeedSampleData(modelBuilder);
        }

        private void SeedSampleData(ModelBuilder modelBuilder)
        {
            // Chi nhánh mẫu
            var chiNhanhId = Guid.NewGuid();
            modelBuilder.Entity<ChiNhanh>().HasData(
                new ChiNhanh
                {
                    ID = chiNhanhId,
                    TenChiNhanh = "QRB Coffee - Chi nhánh chính",
                    MaChiNhanh = "QRB001",
                    IsDelete = false,
                    CreateTime = DateTime.Now
                }
            );

            // Sản phẩm mẫu
            var sanPhamCaPheId = Guid.NewGuid();
            var sanPhamTraId = Guid.NewGuid();
            var sanPhamBanhId = Guid.NewGuid();

            modelBuilder.Entity<SanPham>().HasData(
                new SanPham
                {
                    ID = sanPhamCaPheId,
                    TenSanPham = "Cà phê",
                    MaSanPham = "CF001",
                    IDChiNhanh = chiNhanhId,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                },
                new SanPham
                {
                    ID = sanPhamTraId,
                    TenSanPham = "Trà",
                    MaSanPham = "TR001",
                    IDChiNhanh = chiNhanhId,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                },
                new SanPham
                {
                    ID = sanPhamBanhId,
                    TenSanPham = "Bánh ngọt",
                    MaSanPham = "BN001",
                    IDChiNhanh = chiNhanhId,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                }
            );

            // Loại sản phẩm mẫu
            modelBuilder.Entity<LoaiSanPham>().HasData(
                new LoaiSanPham
                {
                    ID = Guid.NewGuid(),
                    IDSanPham = sanPhamCaPheId,
                    IDChiNhanh = chiNhanhId,
                    TenLoai = "Cà phê đen",
                    MaLoai = "CFD001",
                    DonGia = 25000,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                },
                new LoaiSanPham
                {
                    ID = Guid.NewGuid(),
                    IDSanPham = sanPhamCaPheId,
                    IDChiNhanh = chiNhanhId,
                    TenLoai = "Cà phê sữa",
                    MaLoai = "CFS001",
                    DonGia = 30000,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                },
                new LoaiSanPham
                {
                    ID = Guid.NewGuid(),
                    IDSanPham = sanPhamTraId,
                    IDChiNhanh = chiNhanhId,
                    TenLoai = "Trà sữa",
                    MaLoai = "TS001",
                    DonGia = 35000,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                },
                new LoaiSanPham
                {
                    ID = Guid.NewGuid(),
                    IDSanPham = sanPhamBanhId,
                    IDChiNhanh = chiNhanhId,
                    TenLoai = "Bánh croissant",
                    MaLoai = "BC001",
                    DonGia = 40000,
                    IsDelete = false,
                    CreateTime = DateTime.Now
                }
            );
        }
    }
}
