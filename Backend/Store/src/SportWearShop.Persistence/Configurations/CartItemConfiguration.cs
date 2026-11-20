using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItem", t =>
        {
            t.HasCheckConstraint("CK_CartItem_Quantity", "Quantity > 0");
            t.HasCheckConstraint("CK_CartItem_UnitPrice", "UnitPrice >= 0");
        });

        builder.HasKey(x => x.CartItemId);
        builder.Property(x => x.CartItemId).ValueGeneratedOnAdd();
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Cart>().WithMany()
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CartItem_Cart");
        builder.HasOne<ProductVariant>().WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_CartItem_ProductVariant");
        builder.HasIndex(x => new { x.CartId, x.ProductVariantId }).HasDatabaseName("UX_CartItem_CartId_ProductVariantId").IsUnique();
    }
}
