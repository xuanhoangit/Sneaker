using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakerAPI.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Brand__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand__Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand__Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand__Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand__IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Brand__Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category__Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category__Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Category__Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Color__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Color__Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color__Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Color__Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Size__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Size__Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.Size__Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Tag__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag__Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Tag__Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountClaims_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AccountLogins_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountTokens",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTokens", x => new { x.AccountId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AccountTokens_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInfos",
                columns: table => new
                {
                    CustomerInfo__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerInfo__FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerInfo__LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerInfo__Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerInfo__Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerInfo__TotalSpent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerInfo__SpendingPoint = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerInfo__AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInfos", x => x.CustomerInfo__Id);
                    table.ForeignKey(
                        name: "FK_CustomerInfos_Accounts_CustomerInfo__AccountId",
                        column: x => x.CustomerInfo__AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffInfos",
                columns: table => new
                {
                    StaffInfo__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffInfo__FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffInfo__LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffInfo__Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffInfo__Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffInfo__AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffInfos", x => x.StaffInfo__Id);
                    table.ForeignKey(
                        name: "FK_StaffInfos_Accounts_StaffInfo__AccountId",
                        column: x => x.StaffInfo__AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Product__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product__Sold = table.Column<int>(type: "int", nullable: false),
                    Product__Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Product__Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Product__CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Product__UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Product__Status = table.Column<int>(type: "int", nullable: false),
                    Product__CreatedByAccountId = table.Column<int>(type: "int", nullable: false),
                    Product__BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Product__Id);
                    table.ForeignKey(
                        name: "FK_Products_Accounts_Product__CreatedByAccountId",
                        column: x => x.Product__CreatedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Brands_Product__BrandId",
                        column: x => x.Product__BrandId,
                        principalTable: "Brands",
                        principalColumn: "Brand__Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountRoles",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRoles", x => new { x.AccountId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AccountRoles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Address__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address__AddressDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address__Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address__IsDefault = table.Column<bool>(type: "bit", nullable: true),
                    Address__WardCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address__DistrictId = table.Column<int>(type: "int", nullable: false),
                    Address__ProvinceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address__ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address__CustomerInfo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Address__Id);
                    table.ForeignKey(
                        name: "FK_Addresses_CustomerInfos_Address__CustomerInfo",
                        column: x => x.Address__CustomerInfo,
                        principalTable: "CustomerInfos",
                        principalColumn: "CustomerInfo__Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Favorite__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Favorite__AccountId = table.Column<int>(type: "int", nullable: false),
                    Favorite__ProductId = table.Column<int>(type: "int", nullable: false),
                    Favorite__CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Favorite__IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Favorite__Id);
                    table.ForeignKey(
                        name: "FK_Favorites_Accounts_Favorite__AccountId",
                        column: x => x.Favorite__AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Products_Favorite__ProductId",
                        column: x => x.Favorite__ProductId,
                        principalTable: "Products",
                        principalColumn: "Product__Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductCategory__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCategory__ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductCategory__CategoryId = table.Column<int>(type: "int", nullable: false),
                    Category__Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.ProductCategory__Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories_Category__Id",
                        column: x => x.Category__Id,
                        principalTable: "Categories",
                        principalColumn: "Category__Id");
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products_ProductCategory__ProductId",
                        column: x => x.ProductCategory__ProductId,
                        principalTable: "Products",
                        principalColumn: "Product__Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductColors",
                columns: table => new
                {
                    ProductColor__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductColor__Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductColor__Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductColor__Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductColor__Sold = table.Column<int>(type: "int", nullable: false),
                    ProductColor__Status = table.Column<int>(type: "int", nullable: false),
                    ProductColor__ColorId = table.Column<int>(type: "int", nullable: false),
                    ProductColor__ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColors", x => x.ProductColor__Id);
                    table.ForeignKey(
                        name: "FK_ProductColors_Colors_ProductColor__ColorId",
                        column: x => x.ProductColor__ColorId,
                        principalTable: "Colors",
                        principalColumn: "Color__Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductColors_Products_ProductColor__ProductId",
                        column: x => x.ProductColor__ProductId,
                        principalTable: "Products",
                        principalColumn: "Product__Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTags",
                columns: table => new
                {
                    ProductTag__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTag__ProductId = table.Column<int>(type: "int", nullable: false),
                    Product__Id = table.Column<int>(type: "int", nullable: true),
                    ProductTag__TagId = table.Column<int>(type: "int", nullable: false),
                    Tag__Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTags", x => x.ProductTag__Id);
                    table.ForeignKey(
                        name: "FK_ProductTags_Products_Product__Id",
                        column: x => x.Product__Id,
                        principalTable: "Products",
                        principalColumn: "Product__Id");
                    table.ForeignKey(
                        name: "FK_ProductTags_Tags_Tag__Id",
                        column: x => x.Tag__Id,
                        principalTable: "Tags",
                        principalColumn: "Tag__Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Order__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order__CreatedByAccountId = table.Column<int>(type: "int", nullable: false),
                    Order__Status = table.Column<int>(type: "int", nullable: false),
                    Order__PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    Order__AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Order__PaymentCode = table.Column<long>(type: "bigint", nullable: false),
                    Order__CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Order__PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Order__Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order__AddressId = table.Column<int>(type: "int", nullable: false),
                    OrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Order__Id);
                    table.ForeignKey(
                        name: "FK_Orders_Accounts_Order__CreatedByAccountId",
                        column: x => x.Order__CreatedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_Order__AddressId",
                        column: x => x.Order__AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Address__Id");
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    ProductColorFile__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductColorFile__Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductColorFile__Main = table.Column<bool>(type: "bit", nullable: false),
                    ProductColorFile__ProductColorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.ProductColorFile__Id);
                    table.ForeignKey(
                        name: "FK_Files_ProductColors_ProductColorFile__ProductColorId",
                        column: x => x.ProductColorFile__ProductColorId,
                        principalTable: "ProductColors",
                        principalColumn: "ProductColor__Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductColorSizes",
                columns: table => new
                {
                    ProductColorSize__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductColorSize__Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductColorSize__SizeId = table.Column<int>(type: "int", nullable: false),
                    ProductColorSize__ProductColorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColorSizes", x => x.ProductColorSize__Id);
                    table.ForeignKey(
                        name: "FK_ProductColorSizes_ProductColors_ProductColorSize__ProductColorId",
                        column: x => x.ProductColorSize__ProductColorId,
                        principalTable: "ProductColors",
                        principalColumn: "ProductColor__Id");
                    table.ForeignKey(
                        name: "FK_ProductColorSizes_Sizes_ProductColorSize__SizeId",
                        column: x => x.ProductColorSize__SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Size__Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItem__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartItem__Quantity = table.Column<int>(type: "int", nullable: false),
                    CartItem__CreatedByAccountId = table.Column<int>(type: "int", nullable: false),
                    CartItem__ProductColorSizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItem__Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Accounts_CartItem__CreatedByAccountId",
                        column: x => x.CartItem__CreatedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_ProductColorSizes_CartItem__ProductColorSizeId",
                        column: x => x.CartItem__ProductColorSizeId,
                        principalTable: "ProductColorSizes",
                        principalColumn: "ProductColorSize__Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItem__Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItem__OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderItem__ProductColorSizeId = table.Column<int>(type: "int", nullable: false),
                    OrderItem__Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItem__Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderItem__OrderId",
                        column: x => x.OrderItem__OrderId,
                        principalTable: "Orders",
                        principalColumn: "Order__Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductColorSizes_OrderItem__ProductColorSizeId",
                        column: x => x.OrderItem__ProductColorSizeId,
                        principalTable: "ProductColorSizes",
                        principalColumn: "ProductColorSize__Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountClaims_AccountId",
                table: "AccountClaims",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLogins_AccountId",
                table: "AccountLogins",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRoles_RoleId",
                table: "AccountRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Accounts",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Accounts",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Address__CustomerInfo",
                table: "Addresses",
                column: "Address__CustomerInfo");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartItem__CreatedByAccountId",
                table: "CartItems",
                column: "CartItem__CreatedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartItem__ProductColorSizeId",
                table: "CartItems",
                column: "CartItem__ProductColorSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInfos_CustomerInfo__AccountId",
                table: "CustomerInfos",
                column: "CustomerInfo__AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_Favorite__AccountId",
                table: "Favorites",
                column: "Favorite__AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_Favorite__ProductId",
                table: "Favorites",
                column: "Favorite__ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ProductColorFile__ProductColorId",
                table: "Files",
                column: "ProductColorFile__ProductColorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderItem__OrderId",
                table: "OrderItems",
                column: "OrderItem__OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderItem__ProductColorSizeId",
                table: "OrderItems",
                column: "OrderItem__ProductColorSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order__AddressId",
                table: "Orders",
                column: "Order__AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order__CreatedByAccountId",
                table: "Orders",
                column: "Order__CreatedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order__PaymentCode",
                table: "Orders",
                column: "Order__PaymentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Category__Id",
                table: "ProductCategories",
                column: "Category__Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductCategory__ProductId",
                table: "ProductCategories",
                column: "ProductCategory__ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColors_ProductColor__ColorId",
                table: "ProductColors",
                column: "ProductColor__ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColors_ProductColor__ProductId",
                table: "ProductColors",
                column: "ProductColor__ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColorSizes_ProductColorSize__ProductColorId",
                table: "ProductColorSizes",
                column: "ProductColorSize__ProductColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColorSizes_ProductColorSize__SizeId",
                table: "ProductColorSizes",
                column: "ProductColorSize__SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Product__BrandId",
                table: "Products",
                column: "Product__BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Product__CreatedByAccountId",
                table: "Products",
                column: "Product__CreatedByAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTags_Product__Id",
                table: "ProductTags",
                column: "Product__Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTags_Tag__Id",
                table: "ProductTags",
                column: "Tag__Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StaffInfos_StaffInfo__AccountId",
                table: "StaffInfos",
                column: "StaffInfo__AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountClaims");

            migrationBuilder.DropTable(
                name: "AccountLogins");

            migrationBuilder.DropTable(
                name: "AccountRoles");

            migrationBuilder.DropTable(
                name: "AccountTokens");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "ProductTags");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "StaffInfos");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductColorSizes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "ProductColors");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "CustomerInfos");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
