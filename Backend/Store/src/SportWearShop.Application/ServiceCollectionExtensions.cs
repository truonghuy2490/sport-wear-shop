using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SportWearShop.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        string? mediatRLicenseKey = null)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(assembly);
            options.LicenseKey = mediatRLicenseKey;
        });

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        return services;
    }
}
