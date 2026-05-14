using FluentValidation;
using FluentValidation.AspNetCore;
using SportWearShop.BusinessLogics;
using SportWearShop.BusinessLogics.Validators;
using SportWearShop.BusinessLogics.Validators.Products;

namespace SportWearShop.APIs.DIs
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddBusinessLayer(configuration);

            // validators
            services.AddFluentValidationAutoValidation();
            // products
            services.AddValidatorsFromAssemblyContaining<ProductQueryRequestModelValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateProductRequestModelValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateProductRequestModelValidator>();

            return services;
        }
    }
}
