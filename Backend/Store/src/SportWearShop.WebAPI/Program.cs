using SportWearShop.Persistence;
using SportWearShop.WebAPI;
using SportWearShop.WebAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStorePersistence(builder.Configuration);
builder.Services.AddApiServices();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

if (app.Configuration.GetValue("Web:UseHttpsRedirection", true))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
