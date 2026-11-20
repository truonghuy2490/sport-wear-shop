using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddress", t =>
        {
            t.HasCheckConstraint("CK_UserAddress_DefaultFlags", "IsDefaultShipping IN (0, 1) AND IsDefaultBilling IN (0, 1)");
        });

        builder.HasKey(x => x.UserAddressId);
        builder.Property(x => x.UserAddressId).ValueGeneratedOnAdd();
        builder.Property(x => x.RecipientName).HasMaxLength(150);
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);
        builder.Property(x => x.AddressLine1).HasMaxLength(255);
        builder.Property(x => x.AddressLine2).HasMaxLength(255);
        builder.Property(x => x.Ward).HasMaxLength(100);
        builder.Property(x => x.District).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Province).HasMaxLength(100);
        builder.Property(x => x.PostalCode).HasMaxLength(20);
        builder.Property(x => x.CountryCode).HasMaxLength(10);
        builder.Property(x => x.IsDefaultShipping).HasDefaultValue(false);
        builder.Property(x => x.IsDefaultBilling).HasDefaultValue(false);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Customer>().WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_UserAddress_Customer");
        builder.HasIndex(x => x.CustomerId).HasDatabaseName("IX_UserAddress_CustomerId");
    }
}
