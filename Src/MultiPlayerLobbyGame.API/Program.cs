
using MultiPlayerLobbyGame.API;
using MultiPlayerLobbyGame.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMPLServices();
builder.InitializePod();

var app = builder.Build();

app.Run();