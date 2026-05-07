using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.Services;
using SportWearShop.BussinessLogics.Services;
using SportWearShop.Repositories;
using SportWearShop.Repositories.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRepositoryLayer(configuration);

        
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBrandService, BrandService>();
            
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IProductImageService, ProductImageService>();
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227

        services.AddScoped<IInventoryService, InventoryService>();

        
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();

        services.AddScoped<IAuthService, AuthService>();
<<<<<<< HEAD
=======
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653

        services.AddScoped<IInventoryService, InventoryService>();

        
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();

        services.AddScoped<IAuthService, AuthService>();
>>>>>>> b9a449bbf09be8444339b1e75284695aec3d8227
        services.AddScoped<IUserService, UserService>();
        

        return services;
    }
}