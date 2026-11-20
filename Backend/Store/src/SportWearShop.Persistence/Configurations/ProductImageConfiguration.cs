using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImage");

        builder.HasKey(x => x.ProductImageId);
        builder.Property(x => x.ProductImageId).ValueGeneratedOnAdd();
        builder.Property(x => x.ImageUrl).HasMaxLength(1000);
        builder.Property(x => x.AltText).HasMaxLength(255);
        builder.Property(x => x.SortOrder).HasDefaultValue(0);
        builder.Property(x => x.IsPrimary).HasDefaultValue(false);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Product>().WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ProductImage_Product");
        builder.HasOne<ProductVariant>().WithMany()
            .HasForeignKey(x => new { x.ProductId, x.ProductVariantId })
            .HasPrincipalKey(x => new { x.ProductId, x.ProductVariantId })
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ProductImage_ProductVariant");
    }
}
