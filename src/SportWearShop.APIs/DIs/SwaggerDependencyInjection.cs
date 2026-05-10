using Microsoft.OpenApi;

namespace SportWearShop.APIs.DIs;

public static class SwaggerDependencyInjection
{
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SportWearShop API",
                Version = "v1",
                Description = "API documentation for SportWearShop."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token. Example: Bearer {token}"
            });

            options.AddSecurityRequirement(openApiDocument => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference(
                        "Bearer",
                        openApiDocument),
                    []
                }
            });
        });

        return services;
    }
}