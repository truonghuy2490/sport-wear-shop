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


var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseSwaggerMiddlewares();

app.UseHttpsRedirection();

app.UseAuthenticationMiddlewares();

app.MapControllers();

app.Run();