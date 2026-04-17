using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SportWearShop.APIs.DIs;
using SportWearShop.Repositories;
using SportWearShop.Repositories.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/*builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SportWearShop.Repositories")));*/

// register dependencies from business layer
builder.Services.AddDependencyInjection(builder.Configuration);
/*
builder.Services.AddIdentity<AppUser, AppRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();*/

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SportWearShop API",
        Version = "v1",
        Description = "API for SportWearShop project"
    });
});

var app = builder.Build();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();