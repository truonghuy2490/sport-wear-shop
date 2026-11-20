using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Cart", t =>
        {
            t.HasCheckConstraint("CK_Cart_CartStatus", "CartStatus IN ('ACTIVE', 'CHECKED_OUT', 'ABANDONED')");
        });

        builder.HasKey(x => x.CartId);
        builder.Property(x => x.CartId).ValueGeneratedOnAdd();
        builder.Property(x => x.CartStatus).HasMaxLength(20).HasDefaultValue("ACTIVE");
        builder.Property(x => x.ActiveCustomerId).HasComputedColumnSql("CASE WHEN CartStatus = 'ACTIVE' THEN CustomerId ELSE NULL END", stored: true);
        builder.HasIndex(x => x.ActiveCustomerId).HasDatabaseName("UX_Cart_ActiveCustomer").IsUnique();
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Customer>().WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Cart_Customer");
        builder.HasIndex(x => x.CustomerId).HasDatabaseName("IX_Cart_CustomerId");
    }
}
