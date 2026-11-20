using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer");

        builder.HasKey(x => x.CustomerId);
        builder.Property(x => x.CustomerId).ValueGeneratedOnAdd();
        builder.Property(x => x.IdentityIssuer).HasMaxLength(255).UseCollation("utf8mb4_0900_bin");
        builder.Property(x => x.IdentitySubject).HasMaxLength(255).UseCollation("utf8mb4_0900_bin");
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasIndex(x => new { x.IdentityIssuer, x.IdentitySubject }).HasDatabaseName("UX_Customer_Identity").IsUnique();
    }
}
