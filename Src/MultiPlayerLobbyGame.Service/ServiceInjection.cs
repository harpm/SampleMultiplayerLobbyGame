
using Microsoft.Extensions.DependencyInjection;
using MultiPlayerLobbyGame.Contracts.Repositories;
using MultiPlayerLobbyGame.Contracts.Services;
using MultiPlayerLobbyGame.Data;
using MultiPlayerLobbyGame.Service.LobbyServices;
using MultiPlayerLobbyGame.Service.PlayerServices;
using MultiPlayerLobbyGame.Service.PodServices;

namespace MultiPlayerLobbyGame.Service;

public static class ServiceInjection
{
    public static IServiceCollection AddMPLServices(this IServiceCollection services)
    {
        services.AddSingleton<IPodRepository, PodRepository>();
        services.AddSingleton<IPlayerRepository, PlayerRepository>();
        services.AddSingleton<ILobbyRepository, LobbyRepository>();

        services.AddScoped<IPodService, PodService>();
        services.AddScoped<ILobbyService, LobbyService>();
        services.AddScoped<IPlayerService, PlayerService>();

        return services;
    }
}
