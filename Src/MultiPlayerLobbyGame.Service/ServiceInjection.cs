﻿
using Microsoft.Extensions.DependencyInjection;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Service.LobbyServices;
using MultiPlayerLobbyGame.Service.PlayerServices;
using MultiPlayerLobbyGame.Service.PodServices;

namespace MultiPlayerLobbyGame.Service;

public static class ServiceInjection
{
    public static IServiceCollection AddMPLServices(this IServiceCollection services)
    {
        services.AddScoped<IPodService, PodService>();
        services.AddScoped<ILobbyService, LobbyService>();
        services.AddScoped<IPlayerService, PlayerService>();

        return services;
    }
}
