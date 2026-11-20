using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class ProductRatingConfiguration : IEntityTypeConfiguration<ProductRating>
{
    public void Configure(EntityTypeBuilder<ProductRating> builder)
    {
        builder.ToTable("ProductRating", t =>
        {
            t.HasCheckConstraint("CK_ProductRating_RatingValue", "RatingValue BETWEEN 1 AND 5");
        });

        builder.HasKey(x => x.ProductRatingId);
        builder.Property(x => x.ProductRatingId).ValueGeneratedOnAdd();
        builder.Property(x => x.ReviewText).HasMaxLength(1000);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Product>().WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ProductRating_Product");
        builder.HasOne<Customer>().WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ProductRating_Customer");
        builder.HasIndex(x => new { x.ProductId, x.CustomerId }).HasDatabaseName("UX_ProductRating_Product_Customer").IsUnique();
    }
}
