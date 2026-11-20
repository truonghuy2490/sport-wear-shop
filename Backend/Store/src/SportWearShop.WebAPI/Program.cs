using SportWearShop.Persistence;
using SportWearShop.Application.Features.Brands.GetBrands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddStorePersistence(builder.Configuration);
builder.Services.AddScoped<GetBrandsHandler>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Configuration.GetValue("Web:UseHttpsRedirection", true))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
