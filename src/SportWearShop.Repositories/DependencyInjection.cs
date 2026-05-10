using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Implementations;
using SportWearShop.Repositories.Interfaces;
using SportWearShop.Repositories.Security;
using SportWearShop.Repositories.Security.Interfaces;
<<<<<<< HEAD
using SportWearShop.Repositories.ThirdPartyServices;
using SportWearShop.Repositories.ThirdPartyServices.Implementations;
=======
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
using SportWearShop.Repositories.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoryLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registry Database Context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("SportWearShop.Repositories") // instead of --dir-output
            ));


        // Unit of Work 
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repository Base
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // identity ?
        services.AddIdentity<AppUser, AppRole>(
            options => {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                //options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                //options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // security
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

<<<<<<< HEAD
        // Third party services
        services.AddScoped<ICloudinaryService, CloudinaryService>();
=======
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
        return services;
    }
}
