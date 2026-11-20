using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariant", t =>
        {
            t.HasCheckConstraint("CK_ProductVariant_Status", "Status IN ('ACTIVE', 'INACTIVE')");
            t.HasCheckConstraint("CK_ProductVariant_ListPrice", "ListPrice >= 0");
            t.HasCheckConstraint("CK_ProductVariant_SalePrice", "SalePrice IS NULL OR SalePrice BETWEEN 0 AND ListPrice");
            t.HasCheckConstraint("CK_ProductVariant_WeightGrams", "WeightGrams IS NULL OR WeightGrams >= 0");
        });

        builder.HasKey(x => x.ProductVariantId);
        builder.Property(x => x.ProductVariantId).ValueGeneratedOnAdd();
        builder.Property(x => x.Sku).HasMaxLength(80);
        builder.Property(x => x.ColorCode).HasMaxLength(50);
        builder.Property(x => x.ColorName).HasMaxLength(100);
        builder.Property(x => x.SizeCode).HasMaxLength(30);
        builder.Property(x => x.SizeLabel).HasMaxLength(30);
        builder.Property(x => x.ListPrice).HasPrecision(18, 2);
        builder.Property(x => x.SalePrice).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("ACTIVE");
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasAlternateKey(x => new { x.ProductId, x.ProductVariantId }).HasName("UX_ProductVariant_Product_Variant");
        builder.HasOne<Product>().WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ProductVariant_Product");
        builder.HasIndex(x => x.Sku).HasDatabaseName("UX_ProductVariant_Sku").IsUnique();
        builder.HasIndex(x => new { x.ProductId, x.ColorCode, x.SizeCode }).HasDatabaseName("UX_ProductVariant_Product_Color_Size").IsUnique();
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_ProductVariant_Status");
    }
}
