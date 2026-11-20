using SportWearShop.WebAPI.Configuration;
using SportWearShop.WebAPI.Middlewares;
using SportWearShop.WebAPI.Utilities.Converters;

namespace SportWearShop.WebAPI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableUtcDateTimeJsonConverter());
        });

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddStoreApiVersioning();
        services.AddSwaggerDocumentation();
        return services;
    }
}
