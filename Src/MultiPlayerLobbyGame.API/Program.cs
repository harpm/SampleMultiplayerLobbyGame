
using Microsoft.OpenApi.Models;
using MultiPlayerLobbyGame.API;
using MultiPlayerLobbyGame.API.Middlewares;
using MultiPlayerLobbyGame.Share;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"[DEBUG]: ASPNETCORE_URLS -> {builder.Configuration["ASPNETCORE_URLS"]}");

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" })
);
builder.SetStaticInfo(args);
builder.InjectRequiredServices();

var app = builder.Build();

await app.Services.InitializePod();

if (StaticConfigs.IsInitialized)
{
    app.UseCors(cors =>
    {
        cors.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .Build();
    });

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

    app.UseRouting();
    app.MapControllers();

    app.UseMiddleware<LoadBalanceMiddleware>();
    app.Run();
}
else
{
    throw new Exception("Failed to initialize app...");
}