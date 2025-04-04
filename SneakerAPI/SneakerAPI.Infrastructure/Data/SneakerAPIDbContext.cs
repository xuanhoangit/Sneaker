using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SneakerAPI.Core;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Core.Models.UserEntities;
namespace SneakerAPI.Infrastructure.Data
{
    public class SneakerAPIDbContext : IdentityDbContext<IdentityAccount,IdentityRole<int>,int>
    {
        public SneakerAPIDbContext(DbContextOptions<SneakerAPIDbContext> options):base(options)
        {
            
        }
protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Ví dụ: đổi tên bảng khi dùng kiểu int
        builder.Entity<IdentityAccount>(entity =>
        {
            entity.ToTable("Accounts");
        });
        builder.Entity<IdentityRole<int>>(entity =>
        {
            entity.ToTable("Roles");
        });
        builder.Entity<IdentityUserRole<int>>(entity =>
        {
            entity.ToTable("AccountRoles");
            entity.Property(x=>x.UserId).HasColumnName("AccountId");
        });
        builder.Entity<IdentityUserClaim<int>>(entity =>
        {
            entity.ToTable("AccountClaims");
            entity.Property(x=>x.UserId).HasColumnName("AccountId");
        });
        builder.Entity<IdentityUserLogin<int>>(entity =>
        {
            entity.ToTable("AccountLogins");
            entity.Property(x=>x.UserId).HasColumnName("AccountId");
        });
        builder.Entity<IdentityRoleClaim<int>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });
        builder.Entity<IdentityUserToken<int>>(entity =>
        {
            entity.ToTable("AccountTokens");
            entity.Property(x=>x.UserId).HasColumnName("AccountId");
        });


        ////////////
        builder.Entity<CustomerInfo>()
        .HasIndex(e => e.CustomerInfo__AccountId)
        .IsUnique();
        builder.Entity<StaffInfo>()
        .HasIndex(e => e.StaffInfo__AccountId)
        .IsUnique();
        builder.Entity<Order>()
        .HasIndex(e=>e.Order__PaymentCode)
        .IsUnique();
        /////
        builder.Entity<CartItem>()
            .HasOne(ci => ci.ProductColorSize)
            .WithMany()
            .HasForeignKey(ci => ci.CartItem__ProductColorSizeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ProductColorSize>()
            .HasOne(pcs => pcs.ProductColor)
            .WithMany()
            .HasForeignKey(pcs => pcs.ProductColorSize__ProductColorId)
            .OnDelete(DeleteBehavior.NoAction);
    }
    
        //ORDERENTITIES
        public DbSet<CartItem>? CartItems {get;set;}
        public DbSet<Order>? Orders {get;set;}
        public DbSet<OrderItem>? OrderItems {get;set;}
        //PRODUCTENTITIES
        public DbSet<ProductColorFile> Files {get;set;}
        public DbSet<Product>? Products {get;set;}
        public DbSet<Favorite>? Favorites {get;set;}
        public DbSet<ProductColor>? ProductColors {get;set;}
        public DbSet<ProductColorSize>? ProductColorSizes {get;set;}
        public DbSet<Tag>? Tags {get;set;}
        public DbSet<ProductTag>? ProductTags {get;set;}
        public DbSet<Color>? Colors {get;set;}
        public DbSet<Size>? Sizes {get;set;}
        public DbSet<Category>? Categories {get;set;}
        public DbSet<ProductCategory>? ProductCategories {get;set;}
        public DbSet<Brand>? Brands {get;set;}
        //USERENTITIES

        public DbSet<Address>? Addresses {get;set;}
        public DbSet<CustomerInfo>? CustomerInfos {get;set;}
        public DbSet<StaffInfo>? StaffInfos {get;set;}

    }
}