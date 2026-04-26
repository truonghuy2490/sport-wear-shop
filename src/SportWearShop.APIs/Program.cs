using SportWearShop.APIs.DIs;
using SportWearShop.APIs.ExceptionHandlers;
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


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedDataInitializer.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SportWearShop API v1");
        options.RoutePrefix = "swagger";
    });
}
// Middleware Exception
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();