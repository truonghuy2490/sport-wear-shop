using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories;

public partial class AppDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }

    public virtual DbSet<InventoryStock> InventoryStocks { get; set; }

    public virtual DbSet<OrderHeader> OrderHeaders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductRating> ProductRatings { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brand__DAD4F05EF7B1B364");

            entity.ToTable("Brand");

            entity.HasIndex(e => e.BrandCode, "UX_Brand_BrandCode").IsUnique();

            entity.HasIndex(e => e.BrandName, "UX_Brand_BrandName").IsUnique();
            entity.Property(e => e.BrandImage)
                .HasMaxLength(1000);
            entity.Property(e => e.BrandCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Brand_CreatedAtUtc");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Brand_IsActive");
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Brand_UpdatedAtUtc");
            
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B79EC19504");

            entity.ToTable("Cart");

            entity.HasIndex(e => e.UserId, "IX_Cart_UserId");

            entity.HasIndex(e => new { e.UserId, e.CartStatus }, "UX_Cart_UserId_Active")
                .IsUnique()
                .HasFilter("([CartStatus]='ACTIVE')");

            entity.Property(e => e.CartStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("ACTIVE", "DF_Cart_CartStatus");
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Cart_CreatedAtUtc");
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Cart_UpdatedAtUtc");
            entity.HasOne(d => d.User)
              .WithMany()
              .HasForeignKey(d => d.UserId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0AD5FCE121");

            entity.ToTable("CartItem");

            entity.HasIndex(e => e.CartId, "IX_CartItem_CartId");

            entity.HasIndex(e => new { e.CartId, e.ProductVariantId }, "UX_CartItem_CartId_ProductVariantId").IsUnique();

            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_CartItem_CreatedAtUtc");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_CartItem_UpdatedAtUtc");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_Cart");

            entity.HasOne(d => d.ProductVariant).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_ProductVariant");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0BBD9413BA");

            entity.ToTable("Category");

            entity.HasIndex(e => e.ParentCategoryId, "IX_Category_ParentCategoryId");

            entity.HasIndex(e => e.CategoryCode, "UX_Category_CategoryCode").IsUnique();

            entity.Property(e => e.CategoryCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName).HasMaxLength(150);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Category_CreatedAtUtc");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Category_IsActive");
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Category_UpdatedAtUtc");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("FK_Category_ParentCategory");
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasKey(e => e.InventoryMovementId).HasName("PK__Inventor__DB9462B6B6DAD17B");

            entity.ToTable("InventoryMovement");

            entity.HasIndex(e => e.CreatedAtUtc, "IX_InventoryMovement_CreatedAtUtc");

            entity.HasIndex(e => e.ProductVariantId, "IX_InventoryMovement_ProductVariantId");

            entity.HasIndex(e => new { e.ReferenceType, e.ReferenceId }, "IX_InventoryMovement_ReferenceType_ReferenceId");

            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_InventoryMovement_CreatedAtUtc");
            entity.Property(e => e.MovementType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ProductVariant).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.ProductVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryMovement_ProductVariant");
        });

        modelBuilder.Entity<InventoryStock>(entity =>
        {
            entity.HasKey(e => e.ProductVariantId).HasName("PK__Inventor__E4D6674525F06BCD");

            entity.ToTable("InventoryStock");

            entity.HasIndex(e => e.UpdatedAtUtc, "IX_InventoryStock_UpdatedAtUtc");

            entity.Property(e => e.ProductVariantId).ValueGeneratedNever();
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_InventoryStock_UpdatedAtUtc");

            entity.HasOne(d => d.ProductVariant).WithOne(p => p.InventoryStock)
                .HasForeignKey<InventoryStock>(d => d.ProductVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryStock_ProductVariant");
        });

        modelBuilder.Entity<OrderHeader>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__OrderHea__C3905BCF24410721");

            entity.ToTable("OrderHeader");

            entity.HasIndex(e => e.OrderStatus, "IX_OrderHeader_OrderStatus");

            entity.HasIndex(e => e.OrderedAtUtc, "IX_OrderHeader_OrderedAtUtc");

            entity.HasIndex(e => e.UserId, "IX_OrderHeader_UserId");

            entity.HasIndex(e => e.OrderNumber, "UX_OrderHeader_OrderNumber").IsUnique();

            entity.Property(e => e.CancelledAtUtc).HasPrecision(0);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_OrderHeader_CreatedAtUtc");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("VND", "DF_OrderHeader_CurrencyCode");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PENDING", "DF_OrderHeader_OrderStatus");
            entity.Property(e => e.OrderedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_OrderHeader_OrderedAtUtc");
            entity.Property(e => e.PaidAtUtc).HasPrecision(0);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("UNPAID", "DF_OrderHeader_PaymentStatus");
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SubtotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_OrderHeader_UpdatedAtUtc");
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06817256A78B");

            entity.ToTable("OrderItem");

            entity.HasIndex(e => e.OrderId, "IX_OrderItem_OrderId");

            entity.HasIndex(e => e.ProductVariantId, "IX_OrderItem_ProductVariantId");

            entity.Property(e => e.ColorSnapshot).HasMaxLength(100);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_OrderItem_CreatedAtUtc");
            entity.Property(e => e.ImageUrlSnapshot).HasMaxLength(1000);
            entity.Property(e => e.LineDiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LineTotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductNameSnapshot).HasMaxLength(255);
            entity.Property(e => e.SizeSnapshot).HasMaxLength(30);
            entity.Property(e => e.SkuSnapshot)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_OrderHeader");

            entity.HasOne(d => d.ProductVariant).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductVariantId)
                .HasConstraintName("FK_OrderItem_ProductVariant");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.PaymentTransactionId).HasName("PK__PaymentT__C22AEFE05433880C");

            entity.ToTable("PaymentTransaction");

            entity.HasIndex(e => e.GatewayTransactionRef, "IX_PaymentTransaction_GatewayTransactionRef");

            entity.HasIndex(e => e.OrderId, "IX_PaymentTransaction_OrderId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_PaymentTransaction_CreatedAtUtc");
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.GatewayTransactionRef)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PaidAtUtc).HasPrecision(0);
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TransactionStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_PaymentTransaction_UpdatedAtUtc");

            entity.HasOne(d => d.Order).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentTransaction_OrderHeader");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CD9DD5DBC4");

            entity.ToTable("Product");

            entity.HasIndex(e => e.BrandId, "IX_Product_BrandId");

            entity.HasIndex(e => e.CategoryId, "IX_Product_CategoryId");

            entity.HasIndex(e => e.Status, "IX_Product_Status");

            entity.HasIndex(e => e.ProductCode, "UX_Product_ProductCode").IsUnique();

            entity.HasIndex(e => e.Slug, "UX_Product_Slug").IsUnique();

            entity.Property(e => e.BaseMaterial).HasMaxLength(100);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Product_CreatedAtUtc");
            entity.Property(e => e.Gender)
                .HasConversion<int>()
                .HasDefaultValue(ProductGender.Unisex);
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(ProductStatus.Draft);
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Product_UpdatedAtUtc");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ProductImageId).HasName("PK__ProductI__07B2B1B870EF0B6C");

            entity.ToTable("ProductImage");

            entity.HasIndex(e => e.ProductId, "IX_ProductImage_ProductId");

            entity.HasIndex(e => e.ProductVariantId, "IX_ProductImage_ProductVariantId");

            entity.Property(e => e.AltText).HasMaxLength(255);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_ProductImage_CreatedAtUtc");
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImage_Product");

            entity.HasOne(d => d.ProductVariant).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductVariantId)
                .HasConstraintName("FK_ProductImage_ProductVariant");
        });

        modelBuilder.Entity<ProductRating>(entity =>
        {
            entity.HasKey(e => e.ProductRatingId).HasName("PK__ProductR__2D11B36CFB2C0CD9");

            entity.ToTable("ProductRating");

            entity.HasIndex(e => e.ProductId, "IX_ProductRating_ProductId");

            entity.HasIndex(e => e.UserId, "IX_ProductRating_UserId");

            entity.HasIndex(e => new { e.ProductId, e.UserId }, "UX_ProductRating_Product_User").IsUnique();

            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_ProductRating_CreatedAtUtc");
            entity.Property(e => e.ReviewText).HasMaxLength(1000);
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_ProductRating_UpdatedAtUtc");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductRatings)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductRating_Product");
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.ProductVariantId).HasName("PK__ProductV__E4D66745479449AA");

            entity.ToTable("ProductVariant");

            entity.HasIndex(e => e.ProductId, "IX_ProductVariant_ProductId");

            entity.HasIndex(e => e.Status, "IX_ProductVariant_Status");

            entity.HasIndex(e => new { e.ProductId, e.ColorCode, e.SizeCode }, "UX_ProductVariant_Product_Color_Size").IsUnique();

            entity.HasIndex(e => e.Sku, "UX_ProductVariant_Sku").IsUnique();

            entity.Property(e => e.ColorCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ColorName).HasMaxLength(100);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_ProductVariant_CreatedAtUtc");
            entity.Property(e => e.ListPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SizeCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SizeLabel).HasMaxLength(30);
            entity.Property(e => e.Sku)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(ProductVariantStatus.Draft);
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_ProductVariant_UpdatedAtUtc");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductVariant_Product");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.UserAddressId).HasName("PK__UserAddr__5961BBB77F62F934");

            entity.ToTable("UserAddress");

            entity.HasIndex(e => e.UserId, "IX_UserAddress_UserId");

            entity.Property(e => e.AddressLine1).HasMaxLength(255);
            entity.Property(e => e.AddressLine2).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CountryCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_UserAddress_CreatedAtUtc");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.RecipientName).HasMaxLength(150);
            entity.Property(e => e.UpdatedAtUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_UserAddress_UpdatedAtUtc");
            entity.Property(e => e.Ward).HasMaxLength(100);
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
