using Microsoft.EntityFrameworkCore;
using QRB.Models;
using QRB.Models.Authorization;

namespace QRB.Data
{
    public class QRBDbContext : DbContext
    {
        public QRBDbContext(DbContextOptions<QRBDbContext> options) : base(options)
        {
        }

        // DbSets cho tất cả các bảng
        public DbSet<Department> Departments { get; set; }
        
        // Compatibility property - maps to Department table but returns ChiNhanh objects for backward compatibility
        public IQueryable<ChiNhanh> ChiNhanhs => Departments.Select(d => new ChiNhanh(d));
        
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        
        // New Order tables - chỉ sử dụng Order thay thế hoàn toàn DonHang
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        
        // Note: MaUuDai compatibility should be handled through VoucherService
        // public IQueryable<MaUuDai> MaUuDais => Use VoucherService.GetAllVouchersAsMaUuDaiAsync() instead
        
        public DbSet<Ingredient> Ingredients { get; set; }
        
        // Note: NguyenLieu compatibility should be handled through IngredientService
        // public IQueryable<NguyenLieu> NguyenLieus => Use IngredientService instead
        public DbSet<KhoSanPham> KhoSanPhams { get; set; }
        public DbSet<QuyenSuDung> QuyenSuDungs { get; set; }
        public DbSet<DeXuatMuaSam> DeXuatMuaSams { get; set; }
        public DbSet<ChiTietDonDeXuat> ChiTietDonDeXuats { get; set; }
    public DbSet<Category> Categories { get; set; }
        public DbSet<UserBranch> UserBranches { get; set; }
        
        // Authorization DbSets
        public DbSet<AppRole> Roles { get; set; }
        public DbSet<AppPermission> Permissions { get; set; }
        public DbSet<AppRolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category: map to DB table 'Category'
            modelBuilder.Entity<Category>().ToTable("Category");
            
            // UserBranch configuration
            modelBuilder.Entity<UserBranch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => new { e.UserId, e.BranchId }).IsUnique()
                    .HasDatabaseName("UK_UserBranches_UserId_BranchId");
                
                // Configure relationships without foreign key constraints
                entity.HasOne(e => e.NguoiDung)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.ChiNhanh)
                    .WithMany()
                    .HasForeignKey(e => e.BranchId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            
            base.OnModelCreating(modelBuilder);

            // Cấu hình các quan hệ và ràng buộc
            
            // Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.DepartmentCode).IsUnique();
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

            // Customer (new table)
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.Phone).IsUnique();
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
                
                // Cấu hình các trường mới
                entity.Property(e => e.VaiTro).HasDefaultValue("Staff");
                entity.Property(e => e.TrangThaiHoatDong).HasDefaultValue(true);
                entity.Property(e => e.Email).IsRequired(false);
            });

            // DonHang
            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.MaDonHang).IsUnique();
                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.IDNhanVien);
                entity.HasOne(d => d.ChiNhanh)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.IDChiNhanh);

                // Configure new properties for DonHang
                entity.Property(e => e.TrangThaiThanhToan).HasDefaultValue(false);
                entity.Property(e => e.NgayThanhToan).IsRequired(false);
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

            // Voucher
            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("Voucher");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.IDCustomer).HasColumnName("IDCustomer");
                entity.Property(e => e.VoucherCode).HasColumnName("VoucherCode");
                entity.Property(e => e.Discount).HasColumnName("Discount");
                entity.Property(e => e.Status).HasColumnName("Status");
                entity.Property(e => e.IsDelete).HasColumnName("IsDelete");
                entity.Property(e => e.CreateTime).HasColumnName("CreateTime");
                entity.Property(e => e.UpdateTime).HasColumnName("UpdateTime");
                entity.HasIndex(e => e.VoucherCode).IsUnique();
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Vouchers)
                    .HasForeignKey(d => d.IDCustomer);
            });

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.IDCustomer);
                entity.HasOne(d => d.Employee)
                    .WithMany()
                    .HasForeignKey(d => d.IDEmployee);
                entity.HasOne(d => d.Department)
                    .WithMany()
                    .HasForeignKey(d => d.IDDepartment);
            });

            // OrderDetail
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.IDOrder);
                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.IDProduct);
                entity.HasOne(d => d.ProductType)
                    .WithMany()
                    .HasForeignKey(d => d.IDProductType);
            });

            // Ingredient
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                entity.HasIndex(e => e.IngredientCode).IsUnique();
            });

            // KhoSanPham
            modelBuilder.Entity<KhoSanPham>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).HasDefaultValueSql("NEWID()");
                // Ensure EF maps the FK properties to the exact DB column names
                entity.Property(e => e.IDNguyenLieu).HasColumnName("IDNguyenLieu");
                entity.Property(e => e.IDChiNhanh).HasColumnName("IDChiNhanh");

                // Configure relationships explicitly to avoid EF creating a shadow FK (NguyenLieuID)
                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.KhoSanPhams)
                    .HasForeignKey(d => d.IDNguyenLieu)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ChiNhanh)
                    .WithMany(p => p.KhoSanPhams)
                    .HasForeignKey(d => d.IDChiNhanh)
                    .OnDelete(DeleteBehavior.Restrict);

                // If a shadow property named "NguyenLieuID" exists (from other entity navs), ignore it so EF
                // does not assume an extra column/relationship. This prevents generating/expecting NguyenLieuID.
                entity.Ignore("NguyenLieuID");
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
            });

            // Authorization Tables Configuration
            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired(false);
            });

            modelBuilder.Entity<AppPermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.Module).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired(false);
            });

            modelBuilder.Entity<AppRolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
                entity.Property(e => e.GrantedAt).IsRequired(false);
            });

            // Ignore models that should not be mapped to database
            modelBuilder.Ignore<MaUuDai>();
            // Legacy compatibility class KhachHang should not be mapped - ignore to avoid shadow FK (KhachHangID)
            modelBuilder.Ignore<KhachHang>();
            // Ignore legacy NhomSanPham entity since we use Category now
            modelBuilder.Ignore<NhomSanPham>();
            // Ignore NguyenLieu compatibility class (maps to Ingredient) to avoid duplicate FK mappings
            modelBuilder.Ignore<NguyenLieu>();

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
                    DonGia = "25000",
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
                    DonGia = "30000",
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
                    DonGia = "35000",
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
                    DonGia = "40000",
                    IsDelete = false,
                    CreateTime = DateTime.Now
                }
            );
        }
    }
}
