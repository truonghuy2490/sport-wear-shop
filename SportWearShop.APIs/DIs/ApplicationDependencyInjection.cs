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
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SportWearShop.Repositories")));

            services.AddBusinessLayer(configuration);

            // ???? 
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            


            return services;
        }
    }
}
