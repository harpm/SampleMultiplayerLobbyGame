
using Microsoft.Extensions.DependencyInjection;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Service.Lobby;
using MultiPlayerLobbyGame.Service.Pod;

namespace MultiPlayerLobbyGame.Service;

public static class ServiceInjection
{
    public static IServiceCollection AddMPLServices(this IServiceCollection services)
    {
        services.AddScoped<IPodService, PodService>();
        services.AddScoped<ILobbyService, LobbyService>();
        services.AddScoped<IPlayerService, IPlayerService>();

        return services;
    }
}
