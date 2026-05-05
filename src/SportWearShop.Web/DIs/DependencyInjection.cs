using SportWearShop.Web.Services;
using SportWearShop.Web.Services.Interfaces;

namespace SportWearShop.Web.DIs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // CORS configuration
            services.AddCors(options =>
            {
                options.AddPolicy("AllowRazorPages",
                    builder => builder
                        .WithOrigins("https://localhost:7100")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            // Optional: Keep real API service for later
            services.AddHttpClient<ProductApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5000/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddScoped<IProductApiService, ProductApiClient>();



            return services;
        }
    }
}
