using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Service;
using MultiPlayerLobbyGame.Share;
using MultiPlayerLobbyGame.Share.Utills;
using StackExchange.Redis;

namespace MultiPlayerLobbyGame.API;

internal static class StartUp
{
    public static void InjectRequiredServices(this WebApplicationBuilder builder)
    {
        // TODO: Add IConnectionMultiplexer for redis
        builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
            ConnectionMultiplexer.Connect(StaticConfigs.RedisConnectionString));

        builder.Services.AddMPLServices();
    }

    public static void SetStaticInfo(this WebApplicationBuilder builder)
    {
        StaticConfigs.Ports = builder.GetHostPorts();
        StaticConfigs.PodIP = System.Net.Dns.GetHostName();

        var configs = builder.Configuration;

        StaticConfigs.RedisConnectionString = configs.GetConnectionString(nameof(StaticConfigs.RedisConnectionString));

    }

    public static int[] GetHostPorts(this WebApplicationBuilder builder)
    {
        return builder.Host.Properties.Where(p => (p.Key as Type).Name == "WebHostOptions")
            .SelectMany(p => StringUtils.ExtractPorts((string)p.Value.GetType().GetProperty("ServerUrls").GetValue(p.Value)))
            .ToArray();
    }

    public static Task<bool> InitializePod(this IServiceProvider sp)
    {
        var podService = sp.GetRequiredService<IPodService>();
        return podService.InitializePod(StaticConfigs.PodIP, StaticConfigs.Ports);
    }
}
