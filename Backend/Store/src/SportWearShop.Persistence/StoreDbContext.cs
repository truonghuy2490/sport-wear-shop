using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SportWearShop.Domain.Entities;
using SportWearShop.Domain.Utilities;

namespace SportWearShop.Persistence;

public sealed class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // InnoDB supplies supporting indexes for foreign keys without a declared index.
        configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseCollation("utf8mb4_0900_ai_ci");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetUpdatedTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        SetUpdatedTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetUpdatedTimestamps()
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IUpdatedAtUtc>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.SetUpdatedDateTime(utcNow);
                entry.Property(entity => entity.UpdatedAtUtc).IsModified = true;
            }
        }
    }
}
