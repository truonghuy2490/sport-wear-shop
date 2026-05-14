using SportWearShop.APIs.DIs;
using SportWearShop.APIs.ExceptionHandlers;
using SportWearShop.APIs.Middlewares;
using SportWearShop.Repositories;
using SportWearShop.Repositories.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// OpenAPI + Controller
builder.Services.AddApiServices();

// Application Layer
builder.Services.AddDependencyInjection(builder.Configuration);

// Exception Handling
builder.Services.AddExceptionHandling();

// Swagger
builder.Services.AddSwaggerDocumentation();

// Authen + Author
builder.Services.AddAuthenticationServices(builder.Configuration);

// CORS configuration for SportWearShop.Admin
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAdminCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// seed data
await app.InitializeDatabaseAsync();

// Swagger Configuration
app.UseSwaggerMiddlewares();

// Middleware Exception
app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

// CORS config 
app.UseCors("ReactAdminCors");

// authen + author
app.UseAuthenticationMiddlewares();

app.MapControllers();

app.Run();