
using MultiPlayerLobbyGame.API;
using MultiPlayerLobbyGame.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectRequiredServices();
builder.SetStaticInfo();

var app = builder.Build();

var isInitted = await app.Services.InitializePod();
if (!isInitted)
{
    throw new Exception("Failed to init the pod...");
}
else
{
    app.Run();
}
