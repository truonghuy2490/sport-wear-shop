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

        services.AddScoped<IInventoryService, InventoryService>();

        
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();

        services.AddScoped<IAuthService, AuthService>();
>>>>>>> 0f1984f89c4758af659b95b7677becfbc0e7f653

        services.AddScoped<IInventoryService, InventoryService>();

        
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        

        return services;
    }
}