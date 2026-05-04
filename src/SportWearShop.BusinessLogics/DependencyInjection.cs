using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportWearShop.BusinessLogics.Interfaces;
using SportWearShop.BusinessLogics.Services;
using SportWearShop.Repositories;
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

         services.AddScoped<IProductService, ProductService>();
         services.AddScoped<IOrderService, OrderService>();
         services.AddScoped<ICartService, CartService>();
         services.AddScoped<ICategoryService, CategoryService>();

         services.AddScoped<IAuthService, AuthService>();


        return services;
    }
}