﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SneakerAPI.Infrastructure.Data;

#nullable disable

namespace SneakerAPI.AdminApi.Migrations
{
    [DbContext(typeof(SneakerAPIDbContext))]
    [Migration("20250327150917_27-03-2025_v1")]
    partial class _27032025_v1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("AccountId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AccountClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("AccountId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AccountLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("AccountId");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AccountRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("AccountId");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AccountTokens", (string)null);
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.IdentityAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Accounts", (string)null);
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.OrderEntities.CartItem", b =>
                {
                    b.Property<int>("CartItem__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartItem__Id"));

                    b.Property<int>("CartItem__CreatedByAccountId")
                        .HasColumnType("int");

                    b.Property<int>("CartItem__ProductColorSizeId")
                        .HasColumnType("int");

                    b.Property<int>("CartItem__Quantity")
                        .HasColumnType("int");

                    b.HasKey("CartItem__Id");

                    b.HasIndex("CartItem__CreatedByAccountId");

                    b.HasIndex("CartItem__ProductColorSizeId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.OrderEntities.Order", b =>
                {
                    b.Property<int>("Order__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Order__Id"));

                    b.Property<decimal>("Order__AmountDue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Order__CreatedByAccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Order__CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<long>("Order__PaymentCode")
                        .HasColumnType("bigint");

                    b.Property<int>("Order__PaymentMethod")
                        .HasColumnType("int");

                    b.Property<int>("Order__PaymentStatus")
                        .HasColumnType("int");

                    b.Property<int>("Order__Status")
                        .HasColumnType("int");

                    b.Property<string>("Order__Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Order__Id");

                    b.HasIndex("Order__CreatedByAccountId");

                    b.HasIndex("Order__PaymentCode")
                        .IsUnique();

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.OrderEntities.OrderItem", b =>
                {
                    b.Property<int>("OrderItem__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderItem__Id"));

                    b.Property<int>("OrderItem__OrderId")
                        .HasColumnType("int");

                    b.Property<int>("OrderItem__ProductColorSizeId")
                        .HasColumnType("int");

                    b.Property<int>("OrderItem__Quantity")
                        .HasColumnType("int");

                    b.HasKey("OrderItem__Id");

                    b.HasIndex("OrderItem__OrderId");

                    b.HasIndex("OrderItem__ProductColorSizeId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Brand", b =>
                {
                    b.Property<int>("Brand__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Brand__Id"));

                    b.Property<string>("Brand__Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Brand__IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Brand__Logo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Brand__Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Brand__Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Category", b =>
                {
                    b.Property<int>("Category__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Category__Id"));

                    b.Property<string>("Category__Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category__Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Category__Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Color", b =>
                {
                    b.Property<int>("Color__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Color__Id"));

                    b.Property<string>("Color__Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color__Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Color__Id");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Product", b =>
                {
                    b.Property<int>("Product__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Product__Id"));

                    b.Property<int>("Product__BrandId")
                        .HasColumnType("int");

                    b.Property<int>("Product__CreatedByAccountId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Product__CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Product__Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Product__Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Product__Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Product__UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Product__Id");

                    b.HasIndex("Product__BrandId");

                    b.HasIndex("Product__CreatedByAccountId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductCategory", b =>
                {
                    b.Property<int>("ProductCategory__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductCategory__Id"));

                    b.Property<int?>("Category__Id")
                        .HasColumnType("int");

                    b.Property<int>("ProductCategory__CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("ProductCategory__ProductId")
                        .HasColumnType("int");

                    b.HasKey("ProductCategory__Id");

                    b.HasIndex("Category__Id");

                    b.HasIndex("ProductCategory__ProductId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductColor", b =>
                {
                    b.Property<int>("ProductColor__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductColor__Id"));

                    b.Property<int>("ProductColor__ColorId")
                        .HasColumnType("int");

                    b.Property<string>("ProductColor__Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ProductColor__Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductColor__ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ProductColor__Status")
                        .HasColumnType("int");

                    b.HasKey("ProductColor__Id");

                    b.HasIndex("ProductColor__ColorId");

                    b.HasIndex("ProductColor__ProductId");

                    b.ToTable("ProductColors");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductColorFile", b =>
                {
                    b.Property<int>("ProductColorFile__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductColorFile__Id"));

                    b.Property<string>("ProductColorFile__Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductColorFile__ProductColorId")
                        .HasColumnType("int");

                    b.HasKey("ProductColorFile__Id");

                    b.HasIndex("ProductColorFile__ProductColorId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductColorSize", b =>
                {
                    b.Property<int>("ProductColorSize__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductColorSize__Id"));

                    b.Property<int>("ProductColorSize__ProductColorId")
                        .HasColumnType("int");

                    b.Property<int>("ProductColorSize__Quantity")
                        .HasColumnType("int");

                    b.Property<int>("ProductColorSize__SizeId")
                        .HasColumnType("int");

                    b.HasKey("ProductColorSize__Id");

                    b.HasIndex("ProductColorSize__ProductColorId");

                    b.HasIndex("ProductColorSize__SizeId");

                    b.ToTable("ProductColorSizes");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductTag", b =>
                {
                    b.Property<int>("ProductTag__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductTag__Id"));

                    b.Property<int>("ProductTag__ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ProductTag__TagId")
                        .HasColumnType("int");

                    b.Property<int?>("Product__Id")
                        .HasColumnType("int");

                    b.Property<int?>("Tag__Id")
                        .HasColumnType("int");

                    b.HasKey("ProductTag__Id");

                    b.HasIndex("Product__Id");

                    b.HasIndex("Tag__Id");

                    b.ToTable("ProductTags");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Size", b =>
                {
                    b.Property<int>("Size__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Size__Id"));

                    b.Property<string>("Size__Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Size__Id");

                    b.ToTable("Sizes");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Tag", b =>
                {
                    b.Property<int>("Tag__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Tag__Id"));

                    b.Property<string>("Tag__Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Tag__Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.UserEntities.Address", b =>
                {
                    b.Property<int>("Address__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Address__Id"));

                    b.Property<int>("Address__CustomerInfo")
                        .HasColumnType("int");

                    b.Property<string>("Address__FullAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Address__IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("Address__Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address__ReceiverName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Address__Id");

                    b.HasIndex("Address__CustomerInfo");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.UserEntities.CustomerInfo", b =>
                {
                    b.Property<int>("CustomerInfo__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerInfo__Id"));

                    b.Property<int>("CustomerInfo__AccountId")
                        .HasColumnType("int");

                    b.Property<string>("CustomerInfo__Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerInfo__FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerInfo__LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerInfo__Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("CustomerInfo__SpendingPoint")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("CustomerInfo__TotalSpent")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("CustomerInfo__Id");

                    b.HasIndex("CustomerInfo__AccountId")
                        .IsUnique();

                    b.ToTable("CustomerInfos");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.UserEntities.StaffInfo", b =>
                {
                    b.Property<int>("StaffInfo__Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StaffInfo__Id"));

                    b.Property<int>("StaffInfo__AccountId")
                        .HasColumnType("int");

                    b.Property<string>("StaffInfo__Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StaffInfo__FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StaffInfo__LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StaffInfo__Phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StaffInfo__Id");

                    b.HasIndex("StaffInfo__AccountId")
                        .IsUnique();

                    b.ToTable("StaffInfos");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.OrderEntities.CartItem", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", "Account")
                        .WithMany()
                        .HasForeignKey("CartItem__CreatedByAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.ProductColorSize", "ProductColorSize")
                        .WithMany()
                        .HasForeignKey("CartItem__ProductColorSizeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("ProductColorSize");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.OrderEntities.Order", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", "Account")
                        .WithMany()
                        .HasForeignKey("Order__CreatedByAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.OrderEntities.OrderItem", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.OrderEntities.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderItem__OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.ProductColorSize", "ProductColorSize")
                        .WithMany()
                        .HasForeignKey("OrderItem__ProductColorSizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("ProductColorSize");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Product", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Brand", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("Product__BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", "Account")
                        .WithMany()
                        .HasForeignKey("Product__CreatedByAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Brand");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductCategory", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("Category__Id");

                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductCategory__ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductColor", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Color", "Color")
                        .WithMany()
                        .HasForeignKey("ProductColor__ColorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Product", "Product")
                        .WithMany("ProductColors")
                        .HasForeignKey("ProductColor__ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Color");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductColorFile", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.ProductColor", "ProductColor")
                        .WithMany()
                        .HasForeignKey("ProductColorFile__ProductColorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductColor");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductColorSize", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.ProductColor", "ProductColor")
                        .WithMany()
                        .HasForeignKey("ProductColorSize__ProductColorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Size", "Size")
                        .WithMany()
                        .HasForeignKey("ProductColorSize__SizeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductColor");

                    b.Navigation("Size");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.ProductTag", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("Product__Id");

                    b.HasOne("SneakerAPI.Core.Models.ProductEntities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("Tag__Id");

                    b.Navigation("Product");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.UserEntities.Address", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.UserEntities.CustomerInfo", "CustomerInfo")
                        .WithMany()
                        .HasForeignKey("Address__CustomerInfo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CustomerInfo");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.UserEntities.CustomerInfo", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", "Account")
                        .WithMany()
                        .HasForeignKey("CustomerInfo__AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.UserEntities.StaffInfo", b =>
                {
                    b.HasOne("SneakerAPI.Core.Models.IdentityAccount", "Account")
                        .WithMany()
                        .HasForeignKey("StaffInfo__AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.OrderEntities.Order", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Brand", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Category", b =>
                {
                    b.Navigation("ProductCategories");
                });

            modelBuilder.Entity("SneakerAPI.Core.Models.ProductEntities.Product", b =>
                {
                    b.Navigation("ProductColors");
                });
#pragma warning restore 612, 618
        }
    }
}
