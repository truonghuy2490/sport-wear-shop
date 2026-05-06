using System.Net.Http.Headers;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Services;
using SportWearShop.Web.Services.Implementations;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.DIs;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // api configuration
        var apiBaseUrl = configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("Api BaseUrl is missing.");

        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        // api services
        services.AddScoped<IProductApiService, ProductApiService>();

        return services;
    }
}