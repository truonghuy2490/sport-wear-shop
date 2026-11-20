using Microsoft.EntityFrameworkCore;
using SportWearShop.Persistence.Builders;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Seeders;

internal static class StoreSeeder
{
    public static void Seed(DbContext context)
    {
        var brands = context.Set<Brand>();
        var brand = CreateDemoBrand();

        if (FindExisting(brands, brand).Any())
        {
            return;
        }

        brands.Add(brand);
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        var brands = context.Set<Brand>();
        var brand = CreateDemoBrand();

        if (await FindExisting(brands, brand).AnyAsync(cancellationToken))
        {
            return;
        }

        brands.Add(brand);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static Brand CreateDemoBrand()
        => new BrandBuilder("SWS-DEMO", "SportWearShop Demo").WithActiveStatus(false).Build();

    private static IQueryable<Brand> FindExisting(DbSet<Brand> brands, Brand brand)
        => brands.Where(row => row.BrandCode == brand.BrandCode || row.BrandName == brand.BrandName);
}
