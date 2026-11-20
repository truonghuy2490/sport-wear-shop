using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Constants;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class OrderHeaderConfiguration : IEntityTypeConfiguration<OrderHeader>
{
    public void Configure(EntityTypeBuilder<OrderHeader> builder)
    {
        builder.ToTable("OrderHeader", t =>
        {
            t.HasCheckConstraint("CK_OrderHeader_OrderStatus", "OrderStatus IN ('PENDING', 'CONFIRMED', 'PACKING', 'SHIPPED', 'DELIVERED', 'CANCELLED', 'RETURNED')");
            t.HasCheckConstraint("CK_OrderHeader_PaymentStatus", "PaymentStatus IN ('UNPAID', 'PAID', 'FAILED', 'REFUNDED', 'PARTIALLY_REFUNDED')");
            t.HasCheckConstraint("CK_OrderHeader_SubtotalAmount", "SubtotalAmount >= 0");
            t.HasCheckConstraint("CK_OrderHeader_DiscountAmount", "DiscountAmount >= 0");
            t.HasCheckConstraint("CK_OrderHeader_ShippingFee", "ShippingFee >= 0");
            t.HasCheckConstraint("CK_OrderHeader_TotalAmount", "DiscountAmount <= SubtotalAmount AND TotalAmount = SubtotalAmount - DiscountAmount + ShippingFee");
        });

        builder.HasKey(x => x.OrderId);
        builder.Property(x => x.OrderId).ValueGeneratedOnAdd();
        builder.Property(x => x.OrderNumber).HasMaxLength(30);
        builder.Property(x => x.ShippingAddressSnapshot).HasColumnType("json");
        builder.Property(x => x.BillingAddressSnapshot).HasColumnType("json");
        builder.Property(x => x.OrderStatus).HasMaxLength(20).HasDefaultValue("PENDING");
        builder.Property(x => x.PaymentStatus).HasMaxLength(20).HasDefaultValue("UNPAID");
        builder.Property(x => x.SubtotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2).HasDefaultValue(0);
        builder.Property(x => x.ShippingFee).HasPrecision(18, 2).HasDefaultValue(0);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10).HasDefaultValue(CurrencyCodes.VietnameseDong);
        builder.Property(x => x.OrderedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.PaidAtUtc).HasColumnType("datetime(6)");
        builder.Property(x => x.CancelledAtUtc).HasColumnType("datetime(6)");
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Customer>().WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_OrderHeader_Customer");
        builder.HasIndex(x => x.OrderNumber).HasDatabaseName("UX_OrderHeader_OrderNumber").IsUnique();
        builder.HasIndex(x => new { x.CustomerId, x.OrderedAtUtc, x.OrderId }).HasDatabaseName("IX_OrderHeader_Customer_OrderedAt");
        builder.HasIndex(x => x.OrderedAtUtc).HasDatabaseName("IX_OrderHeader_OrderedAtUtc");
        builder.HasIndex(x => x.OrderStatus).HasDatabaseName("IX_OrderHeader_OrderStatus");
    }
}
