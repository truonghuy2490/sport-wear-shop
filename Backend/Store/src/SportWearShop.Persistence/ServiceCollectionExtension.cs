using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportWearShop.Application.Abstractions.DataAccess;
using SportWearShop.Persistence.DataAccess;
using SportWearShop.Persistence.Seeders;

namespace SportWearShop.Persistence;

public static class ServiceCollectionExtension
{
    private const string ConnectionStringName = "DefaultConnection";

    public static IServiceCollection AddStorePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString) ||
            connectionString == "ConnectionString")
        {
            throw new InvalidOperationException(
                $"Set ConnectionStrings:{ConnectionStringName} before starting Store API.");
        }

        var seedDemoData = bool.TryParse(
            configuration["Store:SeedDemoData"], out var enabled) && enabled;

        services.AddDbContext<StoreDbContext>(options =>
        {
            options.UseMySQL(connectionString);

            if (seedDemoData)
            {
                options.UseSeeding((context, _) => StoreSeeder.Seed(context))
                    .UseAsyncSeeding((context, _, cancellationToken) =>
                        StoreSeeder.SeedAsync(context, cancellationToken));
            }
        });

        services.AddScoped(typeof(IRepository<,>), typeof(EFRepositoryBase<,>));
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();

        return services;
    }
}
