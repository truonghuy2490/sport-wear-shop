using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using SportWearShop.APIs.ExceptionHandlers;
using SportWearShop.Repositories;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.SeedData;

namespace SportWearShop.APIs.Middlewares;


public static class MiddlewareExtensions
{
    // seed db
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        await SeedDataInitializer.SeedAsync(dbContext, roleManager, userManager);

        return app;
    }
    
    // https configuration
    public static WebApplication UseSwaggerMiddlewares(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
        });
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "SportWearShop API v1");
                options.RoutePrefix = "swagger";
            });
        }

        return app;
    }

    // authen + author
    public static WebApplication UseAuthenticationMiddlewares(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}