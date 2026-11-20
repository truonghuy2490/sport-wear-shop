using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brand");

        builder.HasKey(x => x.BrandId);
        builder.Property(x => x.BrandId).ValueGeneratedOnAdd();
        builder.Property(x => x.BrandName).HasMaxLength(100);
        builder.Property(x => x.BrandCode).HasMaxLength(50);
        builder.Property(x => x.IsActive).HasDefaultValue(true).HasSentinel(true);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasIndex(x => x.BrandCode).HasDatabaseName("UX_Brand_BrandCode").IsUnique();
        builder.HasIndex(x => x.BrandName).HasDatabaseName("UX_Brand_BrandName").IsUnique();
    }
}
