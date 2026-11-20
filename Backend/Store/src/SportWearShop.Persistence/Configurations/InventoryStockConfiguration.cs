using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> builder)
    {
        builder.ToTable("InventoryStock", t =>
        {
            t.HasCheckConstraint("CK_InventoryStock_QuantityOnHand", "QuantityOnHand >= 0");
            t.HasCheckConstraint("CK_InventoryStock_QuantityReserved", "QuantityReserved >= 0");
            t.HasCheckConstraint("CK_InventoryStock_Reserved_Not_Greater_Than_OnHand", "QuantityReserved <= QuantityOnHand");
        });

        builder.HasKey(x => x.ProductVariantId);
        builder.Property(x => x.ProductVariantId).ValueGeneratedNever();
        builder.Property(x => x.QuantityOnHand).HasDefaultValue(0);
        builder.Property(x => x.QuantityReserved).HasDefaultValue(0);
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<ProductVariant>().WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_InventoryStock_ProductVariant");
        builder.HasIndex(x => x.UpdatedAtUtc).HasDatabaseName("IX_InventoryStock_UpdatedAtUtc");
    }
}
