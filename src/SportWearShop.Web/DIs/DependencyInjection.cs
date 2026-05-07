<<<<<<< HEAD
﻿using System.Net.Http.Headers;
using SportWearShop.Web.Infrastructure.Api;
using SportWearShop.Web.Infrastructure.HttpHandlers;
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
        // Delegatting Handler: auto attach JWT token into request
        services.AddHttpContextAccessor();
        services.AddTransient<RefreshTokenHandler>();

        // api configuration
        var apiBaseUrl = configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("Api BaseUrl is missing.");

        services.AddHttpClient("SportWearShopApiRaw", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .AddHttpMessageHandler<RefreshTokenHandler>(); // using delegate in here 

        // api services
        services.AddScoped<IProductApiService, ProductApiService>();
        services.AddScoped<IAuthApiService, AuthApiService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // auth service
        services.AddScoped<IAuthCookieService, AuthCookieService>();
        
        
        return services;
    }
}
=======
﻿using SportWearShop.Web.Services;
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
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653
