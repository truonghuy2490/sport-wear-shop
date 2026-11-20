using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovement", t =>
        {
            t.HasCheckConstraint("CK_InventoryMovement_MovementType", "MovementType IN ('IN', 'OUT', 'RESERVE', 'RELEASE', 'ADJUSTMENT')");
            t.HasCheckConstraint("CK_InventoryMovement_Quantity", "(MovementType = 'ADJUSTMENT' AND Quantity <> 0) OR (MovementType <> 'ADJUSTMENT' AND Quantity > 0)");
            t.HasCheckConstraint("CK_InventoryMovement_ReferenceType", "ReferenceType IS NULL OR ReferenceType IN ('ORDER', 'PURCHASE', 'RETURN', 'MANUAL')");
        });

        builder.HasKey(x => x.InventoryMovementId);
        builder.Property(x => x.InventoryMovementId).ValueGeneratedOnAdd();
        builder.Property(x => x.MovementType).HasMaxLength(30);
        builder.Property(x => x.ReferenceType).HasMaxLength(30);
        builder.Property(x => x.Note).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<ProductVariant>().WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_InventoryMovement_ProductVariant");
        builder.HasIndex(x => x.ProductVariantId).HasDatabaseName("IX_InventoryMovement_ProductVariantId");
        builder.HasIndex(x => new { x.ReferenceType, x.ReferenceId }).HasDatabaseName("IX_InventoryMovement_ReferenceType_ReferenceId");
        builder.HasIndex(x => x.CreatedAtUtc).HasDatabaseName("IX_InventoryMovement_CreatedAtUtc");
    }
}
