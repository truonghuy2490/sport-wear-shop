using Microsoft.OpenApi;

namespace SportWearShop.APIs.DIs
{
    public static class SwaggerDependencyInjection
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SportWearShop API",
                    Version = "v1",
                    Description = "API documentation for SportWearShop."
                });
            });

            return services;
        }
    }
}
