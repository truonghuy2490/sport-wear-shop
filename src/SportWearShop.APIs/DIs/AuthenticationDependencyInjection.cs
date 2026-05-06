using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SportWearShop.BussinessLogics.Constants;

namespace SportWearShop.APIs.DIs;

public static class AuthenticationDependencyInjection
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // services add Identity already in Repo DIs XD
        
        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];
        var jwtSecretKey = configuration["Jwt:SecretKey"];

        services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                options => 
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSecretKey!)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                }    
            );

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly",
                policy => policy.RequireRole(AppRoles.Admin));

            options.AddPolicy("StaffOnly",
                policy => policy.RequireRole(AppRoles.Staff));

            options.AddPolicy("CustomerOnly",
                policy => policy.RequireRole(AppRoles.Customer));

            options.AddPolicy("AdminOrStaff",
                policy => policy.RequireRole(AppRoles.Admin, AppRoles.Staff));
        });

        return services;
    }
}