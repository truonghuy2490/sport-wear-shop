using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItem", t =>
        {
            t.HasCheckConstraint("CK_OrderItem_Quantity", "Quantity > 0");
            t.HasCheckConstraint("CK_OrderItem_UnitPrice", "UnitPrice >= 0");
            t.HasCheckConstraint("CK_OrderItem_LineDiscountAmount", "LineDiscountAmount >= 0");
            t.HasCheckConstraint("CK_OrderItem_LineTotalAmount", "LineDiscountAmount <= UnitPrice * Quantity AND LineTotalAmount = UnitPrice * Quantity - LineDiscountAmount");
        });

        builder.HasKey(x => x.OrderItemId);
        builder.Property(x => x.OrderItemId).ValueGeneratedOnAdd();
        builder.Property(x => x.SkuSnapshot).HasMaxLength(80);
        builder.Property(x => x.ProductNameSnapshot).HasMaxLength(255);
        builder.Property(x => x.ColorSnapshot).HasMaxLength(100);
        builder.Property(x => x.SizeSnapshot).HasMaxLength(30);
        builder.Property(x => x.ImageUrlSnapshot).HasMaxLength(1000);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.LineDiscountAmount).HasPrecision(18, 2).HasDefaultValue(0);
        builder.Property(x => x.LineTotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<OrderHeader>().WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_OrderItem_OrderHeader");
        builder.HasOne<ProductVariant>().WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_OrderItem_ProductVariant");
        builder.HasIndex(x => x.OrderId).HasDatabaseName("IX_OrderItem_OrderId");
        builder.HasIndex(x => x.ProductVariantId).HasDatabaseName("IX_OrderItem_ProductVariantId");
    }
}
