using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QRB.Migrations
{
    /// <inheritdoc />
    public partial class TestVoucherModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiNhanh",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenChiNhanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaChiNhanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiNhanh", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    CustomerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaTriDonHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    STT = table.Column<int>(type: "int", nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaTriDonHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    DepartmentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IngredientName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IngredientCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaTriDonHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NguyenLieu",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenNguyenLieu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaNguyenLieu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguyenLieu", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NhomSanPham",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaNhom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenNhom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomSanPham", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuyenSuDung",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    MaQuyen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenQuyen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuyenSuDung", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    TenNguoiDung = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenHienThi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IDChiNhanh = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Staff"),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NguoiDung_Department_IDChiNhanh",
                        column: x => x.IDChiNhanh,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaUuDai",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDKhachHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaGiamGia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TienGiam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiSuDung = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KhachHangID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaUuDai", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaUuDai_CustomerInfo_IDKhachHang",
                        column: x => x.IDKhachHang,
                        principalTable: "CustomerInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaUuDai_KhachHang_KhachHangID",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDCustomer = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Discount = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KhachHangID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Voucher_Customer_IDCustomer",
                        column: x => x.IDCustomer,
                        principalTable: "Customer",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Voucher_KhachHang_KhachHangID",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DeXuatMuaSam",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDNguoiGui = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDChiNhanhGui = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDNguoiNhan = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDChiNhanhNhan = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDeXuat = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDungDeXuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    NoiDungTuChoi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeXuatMuaSam", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeXuatMuaSam_Department_IDChiNhanhGui",
                        column: x => x.IDChiNhanhGui,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeXuatMuaSam_Department_IDChiNhanhNhan",
                        column: x => x.IDChiNhanhNhan,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeXuatMuaSam_NguyenLieu_IDNguoiGui",
                        column: x => x.IDNguoiGui,
                        principalTable: "NguyenLieu",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeXuatMuaSam_NguyenLieu_IDNguoiNhan",
                        column: x => x.IDNguoiNhan,
                        principalTable: "NguyenLieu",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KhoSanPham",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDNguyenLieu = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuongConLai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IDChiNhanh = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguyenLieuID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoSanPham", x => x.ID);
                    table.ForeignKey(
                        name: "FK_KhoSanPham_Department_IDChiNhanh",
                        column: x => x.IDChiNhanh,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KhoSanPham_Ingredient_IDNguyenLieu",
                        column: x => x.IDNguyenLieu,
                        principalTable: "Ingredient",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KhoSanPham_NguyenLieu_NguyenLieuID",
                        column: x => x.NguyenLieuID,
                        principalTable: "NguyenLieu",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    TenSanPham = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaSanPham = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HinhAnh = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdNhomSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IDChiNhanh = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SanPham_Department_IDChiNhanh",
                        column: x => x.IDChiNhanh,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanPham_NhomSanPham_IdNhomSanPham",
                        column: x => x.IdNhomSanPham,
                        principalTable: "NhomSanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDKhachHang = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IDNhanVien = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDChiNhanh = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaDonHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DonGia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaUuDai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TienUuDai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TongTien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiThanhToan = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoBan = table.Column<int>(type: "int", nullable: true),
                    DaTraDon = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHang", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DonHang_Department_IDChiNhanh",
                        column: x => x.IDChiNhanh,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang_IDKhachHang",
                        column: x => x.IDKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DonHang_NguoiDung_IDNhanVien",
                        column: x => x.IDNhanVien,
                        principalTable: "NguoiDung",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungQuyenSuDung",
                columns: table => new
                {
                    NguoiDungsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuyenSuDungsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungQuyenSuDung", x => new { x.NguoiDungsID, x.QuyenSuDungsID });
                    table.ForeignKey(
                        name: "FK_NguoiDungQuyenSuDung_NguoiDung_NguoiDungsID",
                        column: x => x.NguoiDungsID,
                        principalTable: "NguoiDung",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NguoiDungQuyenSuDung_QuyenSuDung_QuyenSuDungsID",
                        column: x => x.QuyenSuDungsID,
                        principalTable: "QuyenSuDung",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDCustomer = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IDEmployee = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDDepartment = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Price = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VoucherCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VoucherPrice = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentStatus = table.Column<bool>(type: "bit", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Table = table.Column<int>(type: "int", nullable: true),
                    Served = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Order_Customer_IDCustomer",
                        column: x => x.IDCustomer,
                        principalTable: "Customer",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Order_Department_IDDepartment",
                        column: x => x.IDDepartment,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_NguoiDung_IDEmployee",
                        column: x => x.IDEmployee,
                        principalTable: "NguoiDung",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBranches_ChiNhanh_BranchId",
                        column: x => x.BranchId,
                        principalTable: "ChiNhanh",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UserBranches_NguoiDung_UserId",
                        column: x => x.UserId,
                        principalTable: "NguoiDung",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonDeXuat",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDDeXuatMuaSam = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDNguyenLieu = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguyenLieuID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonDeXuat", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChiTietDonDeXuat_DeXuatMuaSam_IDDeXuatMuaSam",
                        column: x => x.IDDeXuatMuaSam,
                        principalTable: "DeXuatMuaSam",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonDeXuat_Ingredient_IDNguyenLieu",
                        column: x => x.IDNguyenLieu,
                        principalTable: "Ingredient",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonDeXuat_NguyenLieu_NguyenLieuID",
                        column: x => x.NguyenLieuID,
                        principalTable: "NguyenLieu",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "LoaiSanPham",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenLoai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaLoai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DonGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IDChiNhanh = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiSanPham", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LoaiSanPham_Department_IDChiNhanh",
                        column: x => x.IDChiNhanh,
                        principalTable: "Department",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoaiSanPham_SanPham_IDSanPham",
                        column: x => x.IDSanPham,
                        principalTable: "SanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDDonHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDLoaiSanPham = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThanhTien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHang", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_DonHang_IDDonHang",
                        column: x => x.IDDonHang,
                        principalTable: "DonHang",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_LoaiSanPham_IDLoaiSanPham",
                        column: x => x.IDLoaiSanPham,
                        principalTable: "LoaiSanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_SanPham_IDSanPham",
                        column: x => x.IDSanPham,
                        principalTable: "SanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IDOrder = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDProduct = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDTypeProduct = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderDetail_LoaiSanPham_IDTypeProduct",
                        column: x => x.IDTypeProduct,
                        principalTable: "LoaiSanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_IDOrder",
                        column: x => x.IDOrder,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_SanPham_IDProduct",
                        column: x => x.IDProduct,
                        principalTable: "SanPham",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChiNhanh",
                columns: new[] { "ID", "CreateTime", "IsDelete", "MaChiNhanh", "TenChiNhanh", "UpdateTime" },
                values: new object[] { new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(497), false, "QRB001", "QRB Coffee - Chi nhánh chính", null });

            migrationBuilder.InsertData(
                table: "SanPham",
                columns: new[] { "ID", "CreateTime", "HinhAnh", "IDChiNhanh", "IdNhomSanPham", "IsDelete", "MaSanPham", "NoiDung", "TenSanPham", "UpdateTime" },
                values: new object[,]
                {
                    { new Guid("01ac0027-8632-44e6-b940-a22774f31186"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(612), null, new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new Guid("00000000-0000-0000-0000-000000000000"), false, "BN001", null, "Bánh ngọt", null },
                    { new Guid("16cf86f7-9a78-4878-bdc4-e852863ee46e"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(610), null, new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new Guid("00000000-0000-0000-0000-000000000000"), false, "TR001", null, "Trà", null },
                    { new Guid("cb9f1998-32ae-4841-8c30-191fb54e0fbd"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(608), null, new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new Guid("00000000-0000-0000-0000-000000000000"), false, "CF001", null, "Cà phê", null }
                });

            migrationBuilder.InsertData(
                table: "LoaiSanPham",
                columns: new[] { "ID", "CreateTime", "DonGia", "IDChiNhanh", "IDSanPham", "IsDelete", "MaLoai", "TenLoai", "UpdateTime" },
                values: new object[,]
                {
                    { new Guid("3d4b80ae-d7f7-4433-90a0-10ad44bbdc5c"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(662), "30000", new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new Guid("cb9f1998-32ae-4841-8c30-191fb54e0fbd"), false, "CFS001", "Cà phê sữa", null },
                    { new Guid("84d38c23-1430-426a-9e3c-4bf1f3e407ed"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(677), "40000", new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new Guid("01ac0027-8632-44e6-b940-a22774f31186"), false, "BC001", "Bánh croissant", null },
                    { new Guid("af0a1d29-0c29-4e81-a473-3bb2747be8cb"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(637), "25000", new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new Guid("cb9f1998-32ae-4841-8c30-191fb54e0fbd"), false, "CFD001", "Cà phê đen", null },
                    { new Guid("c3000f64-68ca-4002-a409-408e0d3e7bda"), new DateTime(2025, 8, 19, 21, 58, 41, 176, DateTimeKind.Local).AddTicks(664), "35000", new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"), new Guid("16cf86f7-9a78-4878-bdc4-e852863ee46e"), false, "TS001", "Trà sữa", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDeXuat_IDDeXuatMuaSam",
                table: "ChiTietDonDeXuat",
                column: "IDDeXuatMuaSam");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDeXuat_IDNguyenLieu",
                table: "ChiTietDonDeXuat",
                column: "IDNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDeXuat_NguyenLieuID",
                table: "ChiTietDonDeXuat",
                column: "NguyenLieuID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_IDDonHang",
                table: "ChiTietDonHang",
                column: "IDDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_IDLoaiSanPham",
                table: "ChiTietDonHang",
                column: "IDLoaiSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_IDSanPham",
                table: "ChiTietDonHang",
                column: "IDSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Phone",
                table: "Customer",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_DepartmentCode",
                table: "Department",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeXuatMuaSam_IDChiNhanhGui",
                table: "DeXuatMuaSam",
                column: "IDChiNhanhGui");

            migrationBuilder.CreateIndex(
                name: "IX_DeXuatMuaSam_IDChiNhanhNhan",
                table: "DeXuatMuaSam",
                column: "IDChiNhanhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DeXuatMuaSam_IDNguoiGui",
                table: "DeXuatMuaSam",
                column: "IDNguoiGui");

            migrationBuilder.CreateIndex(
                name: "IX_DeXuatMuaSam_IDNguoiNhan",
                table: "DeXuatMuaSam",
                column: "IDNguoiNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DeXuatMuaSam_MaDeXuat",
                table: "DeXuatMuaSam",
                column: "MaDeXuat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDChiNhanh",
                table: "DonHang",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDKhachHang",
                table: "DonHang",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDNhanVien",
                table: "DonHang",
                column: "IDNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaDonHang",
                table: "DonHang",
                column: "MaDonHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_IngredientCode",
                table: "Ingredient",
                column: "IngredientCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhoSanPham_IDChiNhanh",
                table: "KhoSanPham",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_KhoSanPham_IDNguyenLieu",
                table: "KhoSanPham",
                column: "IDNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_KhoSanPham_NguyenLieuID",
                table: "KhoSanPham",
                column: "NguyenLieuID");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiSanPham_IDChiNhanh",
                table: "LoaiSanPham",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiSanPham_IDSanPham",
                table: "LoaiSanPham",
                column: "IDSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiSanPham_MaLoai",
                table: "LoaiSanPham",
                column: "MaLoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaUuDai_IDKhachHang",
                table: "MaUuDai",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_MaUuDai_KhachHangID",
                table: "MaUuDai",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_IDChiNhanh",
                table: "NguoiDung",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_TenNguoiDung",
                table: "NguoiDung",
                column: "TenNguoiDung",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungQuyenSuDung_QuyenSuDungsID",
                table: "NguoiDungQuyenSuDung",
                column: "QuyenSuDungsID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IDCustomer",
                table: "Order",
                column: "IDCustomer");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IDDepartment",
                table: "Order",
                column: "IDDepartment");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IDEmployee",
                table: "Order",
                column: "IDEmployee");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_IDOrder",
                table: "OrderDetail",
                column: "IDOrder");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_IDProduct",
                table: "OrderDetail",
                column: "IDProduct");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_IDTypeProduct",
                table: "OrderDetail",
                column: "IDTypeProduct");

            migrationBuilder.CreateIndex(
                name: "IX_QuyenSuDung_MaQuyen",
                table: "QuyenSuDung",
                column: "MaQuyen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_IDChiNhanh",
                table: "SanPham",
                column: "IDChiNhanh");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_IdNhomSanPham",
                table: "SanPham",
                column: "IdNhomSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaSanPham",
                table: "SanPham",
                column: "MaSanPham",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_BranchId",
                table: "UserBranches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "UK_UserBranches_UserId_BranchId",
                table: "UserBranches",
                columns: new[] { "UserId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_IDCustomer",
                table: "Voucher",
                column: "IDCustomer");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_KhachHangID",
                table: "Voucher",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_VoucherCode",
                table: "Voucher",
                column: "VoucherCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonDeXuat");

            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "KhoSanPham");

            migrationBuilder.DropTable(
                name: "MaUuDai");

            migrationBuilder.DropTable(
                name: "NguoiDungQuyenSuDung");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserBranches");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "DeXuatMuaSam");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "CustomerInfo");

            migrationBuilder.DropTable(
                name: "QuyenSuDung");

            migrationBuilder.DropTable(
                name: "LoaiSanPham");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ChiNhanh");

            migrationBuilder.DropTable(
                name: "NguyenLieu");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "NhomSanPham");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
