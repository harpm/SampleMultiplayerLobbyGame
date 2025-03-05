
using MultiPlayerLobbyGame.API;
using MultiPlayerLobbyGame.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.SetStaticInfo(args);
builder.InjectRequiredServices();

var app = builder.Build();

app.UseMiddleware<InitializerMiddleware>();
app.Run();

