using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SportWearShop.WebAPI.Configuration;

public sealed class SwaggerGenOptionsSetup(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "SportWearShop Store API",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated ? "This API version is deprecated." : null
            });
        }
    }
}
