using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransaction", t =>
        {
            t.HasCheckConstraint("CK_PaymentTransaction_PaymentMethod", "PaymentMethod IN ('COD', 'MOMO', 'VNPAY', 'CREDIT_CARD', 'BANK_TRANSFER')");
            t.HasCheckConstraint("CK_PaymentTransaction_TransactionStatus", "TransactionStatus IN ('PENDING', 'SUCCESS', 'FAILED', 'CANCELLED', 'REFUNDED')");
            t.HasCheckConstraint("CK_PaymentTransaction_Amount", "Amount >= 0");
        });

        builder.HasKey(x => x.PaymentTransactionId);
        builder.Property(x => x.PaymentTransactionId).ValueGeneratedOnAdd();
        builder.Property(x => x.PaymentMethod).HasMaxLength(30);
        builder.Property(x => x.GatewayTransactionRef).HasMaxLength(100);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.TransactionStatus).HasMaxLength(20);
        builder.Property(x => x.PaidAtUtc).HasColumnType("datetime(6)");
        builder.Property(x => x.FailureReason).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<OrderHeader>().WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_PaymentTransaction_OrderHeader");
        builder.HasIndex(x => x.OrderId).HasDatabaseName("IX_PaymentTransaction_OrderId");
        builder.HasIndex(x => new { x.PaymentMethod, x.GatewayTransactionRef }).HasDatabaseName("UX_PaymentTransaction_Method_Ref").IsUnique();
    }
}
