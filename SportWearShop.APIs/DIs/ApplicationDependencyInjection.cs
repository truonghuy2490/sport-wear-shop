using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportWearShop.BusinessLogics;
using SportWearShop.Repositories; // ?
using SportWearShop.Repositories.Entities; // ?

namespace SportWearShop.APIs.DIs
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddBusinessLayer(configuration);

            // ?
            services.AddIdentityCore<AppUser>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


            return services;
        }
    }
}
