using SportWearShop.BusinessLogics;

namespace SportWearShop.APIs.DIs
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddBusinessLayer(configuration);


            return services;
        }
    }
}
