using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QRB.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCancelRefundFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonDeXuat_DeXuatMuaSam_IDDeXuatMuaSam",
                table: "ChiTietDonDeXuat");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonDeXuat_Ingredient_IDNguyenLieu",
                table: "ChiTietDonDeXuat");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonDeXuat_NguyenLieu_NguyenLieuID",
                table: "ChiTietDonDeXuat");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_LoaiSanPham_IDLoaiSanPham",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_SanPham_IDSanPham",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatMuaSam_Department_IDChiNhanhGui",
                table: "DeXuatMuaSam");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatMuaSam_Department_IDChiNhanhNhan",
                table: "DeXuatMuaSam");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatMuaSam_NguyenLieu_IDNguoiGui",
                table: "DeXuatMuaSam");

            migrationBuilder.DropForeignKey(
                name: "FK_DeXuatMuaSam_NguyenLieu_IDNguoiNhan",
                table: "DeXuatMuaSam");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHang_KhachHang_IDKhachHang",
                table: "DonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHang_NguoiDung_IDNhanVien",
                table: "DonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_LoaiSanPham_Department_IDChiNhanh",
                table: "LoaiSanPham");

            migrationBuilder.DropForeignKey(
                name: "FK_LoaiSanPham_SanPham_IDSanPham",
                table: "LoaiSanPham");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDung_Department_IDChiNhanh",
                table: "NguoiDung");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDungQuyenSuDung_NguoiDung_NguoiDungsID",
                table: "NguoiDungQuyenSuDung");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_NguoiDung_IDEmployee",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_LoaiSanPham_IDTypeProduct",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_SanPham_IDProduct",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_Department_IDChiNhanh",
                table: "SanPham");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_NhomSanPham_IdNhomSanPham",
                table: "SanPham");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranches_NguoiDung_UserId",
                table: "UserBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_KhachHang_KhachHangID",
                table: "Voucher");

            migrationBuilder.DropTable(
                name: "MaUuDai");

            migrationBuilder.DropTable(
                name: "NhomSanPham");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropIndex(
                name: "IX_Voucher_KhachHangID",
                table: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_DonHang_IDKhachHang",
                table: "DonHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NguoiDung",
                table: "NguoiDung");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoaiSanPham",
                table: "LoaiSanPham");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeXuatMuaSam",
                table: "DeXuatMuaSam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChiTietDonDeXuat",
                table: "ChiTietDonDeXuat");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonDeXuat_IDNguyenLieu",
                table: "ChiTietDonDeXuat");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonDeXuat_NguyenLieuID",
                table: "ChiTietDonDeXuat");

            migrationBuilder.DeleteData(
                table: "ChiNhanh",
                keyColumn: "ID",
                keyValue: new Guid("07e4a3b8-5366-4c54-a2e0-c3373ab41ae6"));

            migrationBuilder.DeleteData(
                table: "LoaiSanPham",
                keyColumn: "ID",
                keyValue: new Guid("3d4b80ae-d7f7-4433-90a0-10ad44bbdc5c"));

            migrationBuilder.DeleteData(
                table: "LoaiSanPham",
                keyColumn: "ID",
                keyValue: new Guid("84d38c23-1430-426a-9e3c-4bf1f3e407ed"));

            migrationBuilder.DeleteData(
                table: "LoaiSanPham",
                keyColumn: "ID",
                keyValue: new Guid("af0a1d29-0c29-4e81-a473-3bb2747be8cb"));

            migrationBuilder.DeleteData(
                table: "LoaiSanPham",
                keyColumn: "ID",
                keyValue: new Guid("c3000f64-68ca-4002-a409-408e0d3e7bda"));

            migrationBuilder.DeleteData(
                table: "SanPham",
                keyColumn: "ID",
                keyValue: new Guid("01ac0027-8632-44e6-b940-a22774f31186"));

            migrationBuilder.DeleteData(
                table: "SanPham",
                keyColumn: "ID",
                keyValue: new Guid("16cf86f7-9a78-4878-bdc4-e852863ee46e"));

            migrationBuilder.DeleteData(
                table: "SanPham",
                keyColumn: "ID",
                keyValue: new Guid("cb9f1998-32ae-4841-8c30-191fb54e0fbd"));

            migrationBuilder.DropColumn(
                name: "KhachHangID",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "NguyenLieuID",
                table: "ChiTietDonDeXuat");

            migrationBuilder.RenameTable(
                name: "SanPham",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "NguoiDung",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "LoaiSanPham",
                newName: "TypeProduct");

            migrationBuilder.RenameTable(
                name: "DeXuatMuaSam",
                newName: "Request");

            migrationBuilder.RenameTable(
                name: "ChiTietDonDeXuat",
                newName: "RequestDetail");

            migrationBuilder.RenameColumn(
                name: "TenSanPham",
                table: "Product",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "MaSanPham",
                table: "Product",
                newName: "ProductCode");

            migrationBuilder.RenameColumn(
                name: "IDChiNhanh",
                table: "Product",
                newName: "IDDepartment");

            migrationBuilder.RenameColumn(
                name: "HinhAnh",
                table: "Product",
                newName: "Picture");

            migrationBuilder.RenameColumn(
                name: "IdNhomSanPham",
                table: "Product",
                newName: "IdCategory");

            migrationBuilder.RenameIndex(
                name: "IX_SanPham_MaSanPham",
                table: "Product",
                newName: "IX_Product_ProductCode");

            migrationBuilder.RenameIndex(
                name: "IX_SanPham_IdNhomSanPham",
                table: "Product",
                newName: "IX_Product_IdCategory");

            migrationBuilder.RenameIndex(
                name: "IX_SanPham_IDChiNhanh",
                table: "Product",
                newName: "IX_Product_IDDepartment");

            migrationBuilder.RenameColumn(
                name: "VaiTro",
                table: "User",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "TrangThaiHoatDong",
                table: "User",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TenNguoiDung",
                table: "User",
                newName: "Account");

            migrationBuilder.RenameColumn(
                name: "TenHienThi",
                table: "User",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "MatKhau",
                table: "User",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "IDChiNhanh",
                table: "User",
                newName: "IDDepartment");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_TenNguoiDung",
                table: "User",
                newName: "IX_User_Account");

            migrationBuilder.RenameIndex(
                name: "IX_NguoiDung_IDChiNhanh",
                table: "User",
                newName: "IX_User_IDDepartment");

            migrationBuilder.RenameColumn(
                name: "TenLoai",
                table: "TypeProduct",
                newName: "TypeProductName");

            migrationBuilder.RenameColumn(
                name: "MaLoai",
                table: "TypeProduct",
                newName: "TypeProductCode");

            migrationBuilder.RenameColumn(
                name: "IDSanPham",
                table: "TypeProduct",
                newName: "IDProduct");

            migrationBuilder.RenameColumn(
                name: "IDChiNhanh",
                table: "TypeProduct",
                newName: "IDDepartment");

            migrationBuilder.RenameColumn(
                name: "DonGia",
                table: "TypeProduct",
                newName: "Price");

            migrationBuilder.RenameIndex(
                name: "IX_LoaiSanPham_MaLoai",
                table: "TypeProduct",
                newName: "IX_TypeProduct_TypeProductCode");

            migrationBuilder.RenameIndex(
                name: "IX_LoaiSanPham_IDSanPham",
                table: "TypeProduct",
                newName: "IX_TypeProduct_IDProduct");

            migrationBuilder.RenameIndex(
                name: "IX_LoaiSanPham_IDChiNhanh",
                table: "TypeProduct",
                newName: "IX_TypeProduct_IDDepartment");

            migrationBuilder.RenameColumn(
                name: "TieuDe",
                table: "Request",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "NoiDungTuChoi",
                table: "Request",
                newName: "RejectTitle");

            migrationBuilder.RenameColumn(
                name: "NoiDungDeXuat",
                table: "Request",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "MaDeXuat",
                table: "Request",
                newName: "RequestCode");

            migrationBuilder.RenameColumn(
                name: "IDNguoiNhan",
                table: "Request",
                newName: "IDReceiver");

            migrationBuilder.RenameColumn(
                name: "IDNguoiGui",
                table: "Request",
                newName: "IDSender");

            migrationBuilder.RenameColumn(
                name: "IDChiNhanhNhan",
                table: "Request",
                newName: "IDReceiveDepartment");

            migrationBuilder.RenameColumn(
                name: "IDChiNhanhGui",
                table: "Request",
                newName: "IDSenDDepartment");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatMuaSam_MaDeXuat",
                table: "Request",
                newName: "IX_Request_RequestCode");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatMuaSam_IDNguoiNhan",
                table: "Request",
                newName: "IX_Request_IDReceiver");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatMuaSam_IDNguoiGui",
                table: "Request",
                newName: "IX_Request_IDSender");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatMuaSam_IDChiNhanhNhan",
                table: "Request",
                newName: "IX_Request_IDReceiveDepartment");

            migrationBuilder.RenameIndex(
                name: "IX_DeXuatMuaSam_IDChiNhanhGui",
                table: "Request",
                newName: "IX_Request_IDSenDDepartment");

            migrationBuilder.RenameColumn(
                name: "SoLuong",
                table: "RequestDetail",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "IDDeXuatMuaSam",
                table: "RequestDetail",
                newName: "IDRequest");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDonDeXuat_IDDeXuatMuaSam",
                table: "RequestDetail",
                newName: "IX_RequestDetail_IDRequest");

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDate",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Order",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefunded",
                table: "Order",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundAmount",
                table: "Order",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RefundApprovedByUserId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundDate",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypeProduct",
                table: "TypeProduct",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Request",
                table: "Request",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestDetail",
                table: "RequestDetail",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "ChiNhanh",
                columns: new[] { "ID", "CreateTime", "IsDelete", "MaChiNhanh", "TenChiNhanh", "UpdateTime" },
                values: new object[] { new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5687), false, "QRB001", "QRB Coffee - Chi nhánh chính", null });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ID", "CreateTime", "Picture", "IDDepartment", "IdCategory", "IsDelete", "ProductCode", "NoiDung", "ProductName", "UpdateTime" },
                values: new object[,]
                {
                    { new Guid("302c11c6-9b01-48fa-adb8-ae249c95198f"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5815), null, new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new Guid("00000000-0000-0000-0000-000000000000"), false, "CF001", null, "Cà phê", null },
                    { new Guid("897f6f94-7451-47b6-ac74-f6d289203a0d"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5817), null, new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new Guid("00000000-0000-0000-0000-000000000000"), false, "TR001", null, "Trà", null },
                    { new Guid("a5395ab3-c888-4deb-97f1-499233770565"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5819), null, new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new Guid("00000000-0000-0000-0000-000000000000"), false, "BN001", null, "Bánh ngọt", null }
                });

            migrationBuilder.InsertData(
                table: "TypeProduct",
                columns: new[] { "ID", "CreateTime", "Price", "IDDepartment", "IDProduct", "IsDelete", "TypeProductCode", "TypeProductName", "UpdateTime" },
                values: new object[,]
                {
                    { new Guid("026b2bac-6578-46b3-b8e0-efce45bdcb43"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5844), "35000", new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new Guid("897f6f94-7451-47b6-ac74-f6d289203a0d"), false, "TS001", "Trà sữa", null },
                    { new Guid("3b5e947f-ba65-46bc-b22b-2aaaeb9a5369"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5846), "40000", new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new Guid("a5395ab3-c888-4deb-97f1-499233770565"), false, "BC001", "Bánh croissant", null },
                    { new Guid("4e9fef51-45b4-4e8e-932a-c0269202e4bc"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5839), "25000", new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new Guid("302c11c6-9b01-48fa-adb8-ae249c95198f"), false, "CFD001", "Cà phê đen", null },
                    { new Guid("e3561186-5250-499f-9ed6-9c2184dd4dfe"), new DateTime(2025, 9, 11, 0, 9, 18, 351, DateTimeKind.Local).AddTicks(5842), "30000", new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"), new Guid("302c11c6-9b01-48fa-adb8-ae249c95198f"), false, "CFS001", "Cà phê sữa", null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_Product_IDSanPham",
                table: "ChiTietDonHang",
                column: "IDSanPham",
                principalTable: "Product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_TypeProduct_IDLoaiSanPham",
                table: "ChiTietDonHang",
                column: "IDLoaiSanPham",
                principalTable: "TypeProduct",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHang_User_IDNhanVien",
                table: "DonHang",
                column: "IDNhanVien",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDungQuyenSuDung_User_NguoiDungsID",
                table: "NguoiDungQuyenSuDung",
                column: "NguoiDungsID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_IDEmployee",
                table: "Order",
                column: "IDEmployee",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Product_IDProduct",
                table: "OrderDetail",
                column: "IDProduct",
                principalTable: "Product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_TypeProduct_IDTypeProduct",
                table: "OrderDetail",
                column: "IDTypeProduct",
                principalTable: "TypeProduct",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_IdCategory",
                table: "Product",
                column: "IdCategory",
                principalTable: "Category",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Department_IDDepartment",
                table: "Product",
                column: "IDDepartment",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Department_IDReceiveDepartment",
                table: "Request",
                column: "IDReceiveDepartment",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Department_IDSenDDepartment",
                table: "Request",
                column: "IDSenDDepartment",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_NguyenLieu_IDReceiver",
                table: "Request",
                column: "IDReceiver",
                principalTable: "NguyenLieu",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_NguyenLieu_IDSender",
                table: "Request",
                column: "IDSender",
                principalTable: "NguyenLieu",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestDetail_Request_IDRequest",
                table: "RequestDetail",
                column: "IDRequest",
                principalTable: "Request",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TypeProduct_Department_IDDepartment",
                table: "TypeProduct",
                column: "IDDepartment",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TypeProduct_Product_IDProduct",
                table: "TypeProduct",
                column: "IDProduct",
                principalTable: "Product",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Department_IDDepartment",
                table: "User",
                column: "IDDepartment",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranches_User_UserId",
                table: "UserBranches",
                column: "UserId",
                principalTable: "User",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_Product_IDSanPham",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_TypeProduct_IDLoaiSanPham",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_DonHang_User_IDNhanVien",
                table: "DonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDungQuyenSuDung_User_NguoiDungsID",
                table: "NguoiDungQuyenSuDung");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_IDEmployee",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Product_IDProduct",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_TypeProduct_IDTypeProduct",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_IdCategory",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Department_IDDepartment",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Department_IDReceiveDepartment",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Department_IDSenDDepartment",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_NguyenLieu_IDReceiver",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_NguyenLieu_IDSender",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestDetail_Request_IDRequest",
                table: "RequestDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_TypeProduct_Department_IDDepartment",
                table: "TypeProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_TypeProduct_Product_IDProduct",
                table: "TypeProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Department_IDDepartment",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranches_User_UserId",
                table: "UserBranches");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypeProduct",
                table: "TypeProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestDetail",
                table: "RequestDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Request",
                table: "Request");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DeleteData(
                table: "ChiNhanh",
                keyColumn: "ID",
                keyValue: new Guid("3c43a4c8-6612-462b-9da0-dcdd47684e38"));

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "ID",
                keyValue: new Guid("026b2bac-6578-46b3-b8e0-efce45bdcb43"));

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "ID",
                keyValue: new Guid("3b5e947f-ba65-46bc-b22b-2aaaeb9a5369"));

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "ID",
                keyValue: new Guid("4e9fef51-45b4-4e8e-932a-c0269202e4bc"));

            migrationBuilder.DeleteData(
                table: "TypeProduct",
                keyColumn: "ID",
                keyValue: new Guid("e3561186-5250-499f-9ed6-9c2184dd4dfe"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ID",
                keyValue: new Guid("302c11c6-9b01-48fa-adb8-ae249c95198f"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ID",
                keyValue: new Guid("897f6f94-7451-47b6-ac74-f6d289203a0d"));

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ID",
                keyValue: new Guid("a5395ab3-c888-4deb-97f1-499233770565"));

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelledDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsRefunded",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundApprovedByUserId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "Order");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "NguoiDung");

            migrationBuilder.RenameTable(
                name: "TypeProduct",
                newName: "LoaiSanPham");

            migrationBuilder.RenameTable(
                name: "RequestDetail",
                newName: "ChiTietDonDeXuat");

            migrationBuilder.RenameTable(
                name: "Request",
                newName: "DeXuatMuaSam");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "SanPham");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "NguoiDung",
                newName: "TenHienThi");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "NguoiDung",
                newName: "TrangThaiHoatDong");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "NguoiDung",
                newName: "VaiTro");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "NguoiDung",
                newName: "MatKhau");

            migrationBuilder.RenameColumn(
                name: "IDDepartment",
                table: "NguoiDung",
                newName: "IDChiNhanh");

            migrationBuilder.RenameColumn(
                name: "Account",
                table: "NguoiDung",
                newName: "TenNguoiDung");

            migrationBuilder.RenameIndex(
                name: "IX_User_IDDepartment",
                table: "NguoiDung",
                newName: "IX_NguoiDung_IDChiNhanh");

            migrationBuilder.RenameIndex(
                name: "IX_User_Account",
                table: "NguoiDung",
                newName: "IX_NguoiDung_TenNguoiDung");

            migrationBuilder.RenameColumn(
                name: "TypeProductName",
                table: "LoaiSanPham",
                newName: "TenLoai");

            migrationBuilder.RenameColumn(
                name: "TypeProductCode",
                table: "LoaiSanPham",
                newName: "MaLoai");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "LoaiSanPham",
                newName: "DonGia");

            migrationBuilder.RenameColumn(
                name: "IDProduct",
                table: "LoaiSanPham",
                newName: "IDSanPham");

            migrationBuilder.RenameColumn(
                name: "IDDepartment",
                table: "LoaiSanPham",
                newName: "IDChiNhanh");

            migrationBuilder.RenameIndex(
                name: "IX_TypeProduct_TypeProductCode",
                table: "LoaiSanPham",
                newName: "IX_LoaiSanPham_MaLoai");

            migrationBuilder.RenameIndex(
                name: "IX_TypeProduct_IDProduct",
                table: "LoaiSanPham",
                newName: "IX_LoaiSanPham_IDSanPham");

            migrationBuilder.RenameIndex(
                name: "IX_TypeProduct_IDDepartment",
                table: "LoaiSanPham",
                newName: "IX_LoaiSanPham_IDChiNhanh");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ChiTietDonDeXuat",
                newName: "SoLuong");

            migrationBuilder.RenameColumn(
                name: "IDRequest",
                table: "ChiTietDonDeXuat",
                newName: "IDDeXuatMuaSam");

            migrationBuilder.RenameIndex(
                name: "IX_RequestDetail_IDRequest",
                table: "ChiTietDonDeXuat",
                newName: "IX_ChiTietDonDeXuat_IDDeXuatMuaSam");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "DeXuatMuaSam",
                newName: "TieuDe");

            migrationBuilder.RenameColumn(
                name: "RequestCode",
                table: "DeXuatMuaSam",
                newName: "MaDeXuat");

            migrationBuilder.RenameColumn(
                name: "RejectTitle",
                table: "DeXuatMuaSam",
                newName: "NoiDungTuChoi");

            migrationBuilder.RenameColumn(
                name: "IDSender",
                table: "DeXuatMuaSam",
                newName: "IDNguoiGui");

            migrationBuilder.RenameColumn(
                name: "IDSenDDepartment",
                table: "DeXuatMuaSam",
                newName: "IDChiNhanhGui");

            migrationBuilder.RenameColumn(
                name: "IDReceiver",
                table: "DeXuatMuaSam",
                newName: "IDNguoiNhan");

            migrationBuilder.RenameColumn(
                name: "IDReceiveDepartment",
                table: "DeXuatMuaSam",
                newName: "IDChiNhanhNhan");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "DeXuatMuaSam",
                newName: "NoiDungDeXuat");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestCode",
                table: "DeXuatMuaSam",
                newName: "IX_DeXuatMuaSam_MaDeXuat");

            migrationBuilder.RenameIndex(
                name: "IX_Request_IDSender",
                table: "DeXuatMuaSam",
                newName: "IX_DeXuatMuaSam_IDNguoiGui");

            migrationBuilder.RenameIndex(
                name: "IX_Request_IDSenDDepartment",
                table: "DeXuatMuaSam",
                newName: "IX_DeXuatMuaSam_IDChiNhanhGui");

            migrationBuilder.RenameIndex(
                name: "IX_Request_IDReceiver",
                table: "DeXuatMuaSam",
                newName: "IX_DeXuatMuaSam_IDNguoiNhan");

            migrationBuilder.RenameIndex(
                name: "IX_Request_IDReceiveDepartment",
                table: "DeXuatMuaSam",
                newName: "IX_DeXuatMuaSam_IDChiNhanhNhan");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "SanPham",
                newName: "TenSanPham");

            migrationBuilder.RenameColumn(
                name: "ProductCode",
                table: "SanPham",
                newName: "MaSanPham");

            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "SanPham",
                newName: "HinhAnh");

            migrationBuilder.RenameColumn(
                name: "IDDepartment",
                table: "SanPham",
                newName: "IDChiNhanh");

            migrationBuilder.RenameColumn(
                name: "IdCategory",
                table: "SanPham",
                newName: "IdNhomSanPham");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ProductCode",
                table: "SanPham",
                newName: "IX_SanPham_MaSanPham");

            migrationBuilder.RenameIndex(
                name: "IX_Product_IDDepartment",
                table: "SanPham",
                newName: "IX_SanPham_IDChiNhanh");

            migrationBuilder.RenameIndex(
                name: "IX_Product_IdCategory",
                table: "SanPham",
                newName: "IX_SanPham_IdNhomSanPham");

            migrationBuilder.AddColumn<Guid>(
                name: "KhachHangID",
                table: "Voucher",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NguyenLieuID",
                table: "ChiTietDonDeXuat",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NguoiDung",
                table: "NguoiDung",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoaiSanPham",
                table: "LoaiSanPham",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChiTietDonDeXuat",
                table: "ChiTietDonDeXuat",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeXuatMuaSam",
                table: "DeXuatMuaSam",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SanPham",
                table: "SanPham",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaTriDonHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NhomSanPham",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    MaNhom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenNhom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomSanPham", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MaUuDai",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDKhachHang = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    KhachHangID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaGiamGia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TienGiam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiSuDung = table.Column<bool>(type: "bit", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "IX_Voucher_KhachHangID",
                table: "Voucher",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_IDKhachHang",
                table: "DonHang",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDeXuat_IDNguyenLieu",
                table: "ChiTietDonDeXuat",
                column: "IDNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDeXuat_NguyenLieuID",
                table: "ChiTietDonDeXuat",
                column: "NguyenLieuID");

            migrationBuilder.CreateIndex(
                name: "IX_MaUuDai_IDKhachHang",
                table: "MaUuDai",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_MaUuDai_KhachHangID",
                table: "MaUuDai",
                column: "KhachHangID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonDeXuat_DeXuatMuaSam_IDDeXuatMuaSam",
                table: "ChiTietDonDeXuat",
                column: "IDDeXuatMuaSam",
                principalTable: "DeXuatMuaSam",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonDeXuat_Ingredient_IDNguyenLieu",
                table: "ChiTietDonDeXuat",
                column: "IDNguyenLieu",
                principalTable: "Ingredient",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonDeXuat_NguyenLieu_NguyenLieuID",
                table: "ChiTietDonDeXuat",
                column: "NguyenLieuID",
                principalTable: "NguyenLieu",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_LoaiSanPham_IDLoaiSanPham",
                table: "ChiTietDonHang",
                column: "IDLoaiSanPham",
                principalTable: "LoaiSanPham",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_SanPham_IDSanPham",
                table: "ChiTietDonHang",
                column: "IDSanPham",
                principalTable: "SanPham",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatMuaSam_Department_IDChiNhanhGui",
                table: "DeXuatMuaSam",
                column: "IDChiNhanhGui",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatMuaSam_Department_IDChiNhanhNhan",
                table: "DeXuatMuaSam",
                column: "IDChiNhanhNhan",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatMuaSam_NguyenLieu_IDNguoiGui",
                table: "DeXuatMuaSam",
                column: "IDNguoiGui",
                principalTable: "NguyenLieu",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeXuatMuaSam_NguyenLieu_IDNguoiNhan",
                table: "DeXuatMuaSam",
                column: "IDNguoiNhan",
                principalTable: "NguyenLieu",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DonHang_KhachHang_IDKhachHang",
                table: "DonHang",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DonHang_NguoiDung_IDNhanVien",
                table: "DonHang",
                column: "IDNhanVien",
                principalTable: "NguoiDung",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoaiSanPham_Department_IDChiNhanh",
                table: "LoaiSanPham",
                column: "IDChiNhanh",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoaiSanPham_SanPham_IDSanPham",
                table: "LoaiSanPham",
                column: "IDSanPham",
                principalTable: "SanPham",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDung_Department_IDChiNhanh",
                table: "NguoiDung",
                column: "IDChiNhanh",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NguoiDungQuyenSuDung_NguoiDung_NguoiDungsID",
                table: "NguoiDungQuyenSuDung",
                column: "NguoiDungsID",
                principalTable: "NguoiDung",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_NguoiDung_IDEmployee",
                table: "Order",
                column: "IDEmployee",
                principalTable: "NguoiDung",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_LoaiSanPham_IDTypeProduct",
                table: "OrderDetail",
                column: "IDTypeProduct",
                principalTable: "LoaiSanPham",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_SanPham_IDProduct",
                table: "OrderDetail",
                column: "IDProduct",
                principalTable: "SanPham",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_Department_IDChiNhanh",
                table: "SanPham",
                column: "IDChiNhanh",
                principalTable: "Department",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_NhomSanPham_IdNhomSanPham",
                table: "SanPham",
                column: "IdNhomSanPham",
                principalTable: "NhomSanPham",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranches_NguoiDung_UserId",
                table: "UserBranches",
                column: "UserId",
                principalTable: "NguoiDung",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_KhachHang_KhachHangID",
                table: "Voucher",
                column: "KhachHangID",
                principalTable: "KhachHang",
                principalColumn: "ID");
        }
    }
}
