using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.APIs.ExceptionHandlers;

namespace SportWearShop.APIs.DIs
{
    public static class ExceptionHandlingDependencyInjection
    {
        public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            return services;
        }
    }

}
