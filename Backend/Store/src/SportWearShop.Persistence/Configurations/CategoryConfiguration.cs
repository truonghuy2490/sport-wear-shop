using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(x => x.CategoryId);
        builder.Property(x => x.CategoryId).ValueGeneratedOnAdd();
        builder.Property(x => x.CategoryName).HasMaxLength(150);
        builder.Property(x => x.CategoryCode).HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasDefaultValue(0);
        builder.Property(x => x.IsActive).HasDefaultValue(true).HasSentinel(true);
        builder.Property(x => x.CreatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.Property(x => x.UpdatedAtUtc).HasColumnType("datetime(6)").HasDefaultValueSql("(UTC_TIMESTAMP(6))");
        builder.HasOne<Category>().WithMany()
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Category_ParentCategory");
        builder.HasIndex(x => x.CategoryCode).HasDatabaseName("UX_Category_CategoryCode").IsUnique();
        builder.HasIndex(x => x.ParentCategoryId).HasDatabaseName("IX_Category_ParentCategoryId");
    }
}
