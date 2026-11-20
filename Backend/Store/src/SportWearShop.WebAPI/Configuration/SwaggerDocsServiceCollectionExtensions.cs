using Asp.Versioning.ApiExplorer;

namespace SportWearShop.WebAPI.Configuration;

public static class SwaggerDocsServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => options.EnableAnnotations());
        services.ConfigureOptions<SwaggerGenOptionsSetup>();
        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwagger(options => options.RouteTemplate = "openapi/{documentName}.json");

        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"./{description.GroupName}/swagger.json",
                    $"SportWearShop Store API {description.GroupName.ToUpperInvariant()}");
            }
        });

        return app;
    }
}
