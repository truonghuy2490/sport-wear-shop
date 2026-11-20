using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product", t =>
        {
            t.HasCheckConstraint("CK_Product_Status", "Status IN ('DRAFT', 'ACTIVE', 'INACTIVE', 'DISCONTINUED')");
            t.HasCheckConstraint("CK_Product_Gender", "Gender IS NULL OR Gender IN ('MEN', 'WOMEN', 'UNISEX', 'KIDS')");
        });

        builder.HasKey(x => x.ProductId);
        builder.Property(x => x.ProductId).ValueGeneratedOnAdd();
        builder.Property(x => x.ProductName).HasMaxLength(255);
        builder.Property(x => x.ProductCode).HasMaxLength(50);
        builder.Property(x => x.Slug).HasMaxLength(255);
        builder.Property(x => x.Description).HasColumnType("longtext");
        builder.Property(x => x.Gender).HasMaxLength(20);
        builder.Property(x => x.BaseMaterial).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("ACTIVE");
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Brand>().WithMany()
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Product_Brand");
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Product_Category");
        builder.HasIndex(x => x.ProductCode).HasDatabaseName("UX_Product_ProductCode").IsUnique();
        builder.HasIndex(x => x.Slug).HasDatabaseName("UX_Product_Slug").IsUnique();
        builder.HasIndex(x => x.BrandId).HasDatabaseName("IX_Product_BrandId");
        builder.HasIndex(x => x.CategoryId).HasDatabaseName("IX_Product_CategoryId");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_Product_Status");
    }
}
