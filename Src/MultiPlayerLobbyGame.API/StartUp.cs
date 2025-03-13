using System.Net;
using System.Net.Sockets;
using StackExchange.Redis;
using MultiPlayerLobbyGame.Service;
using MultiPlayerLobbyGame.Share;
using MultiPlayerLobbyGame.Share.Utills;
using MultiPlayerLobbyGame.Contracts.Services;

namespace MultiPlayerLobbyGame.API;

internal static class StartUp
{
    public static void InjectRequiredServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
            ConnectionMultiplexer.Connect(StaticConfigs.RedisConnectionString));

        builder.Services.AddMPLServices();

        builder.Services.AddHttpClient();
    }

    public static void SetStaticInfo(this WebApplicationBuilder builder, string[] args)
    {
        if (!builder.Environment.IsDevelopment())
        {
            StaticConfigs.RedisConnectionString =
                Environment.GetEnvironmentVariable("RedisConnection");
        }
        else
        {
            var configs = builder.Configuration;
            StaticConfigs.RedisConnectionString = 
                configs.GetConnectionString(nameof(StaticConfigs.RedisConnectionString));
        }
        Console.WriteLine($"[DEBUG]: RedisConn -> {StaticConfigs.RedisConnectionString}");

        var entry = Dns.GetHostEntry(Dns.GetHostName());
        StaticConfigs.PodIP = entry.AddressList
            .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
            .Select(a => a.ToString())
            .FirstOrDefault();

        Console.WriteLine($"[DEBUG] IP -> {StaticConfigs.PodIP}");

        StaticConfigs.Ports =
            StringUtils.ExtractPorts(builder.Configuration["ASPNETCORE_URLS"]);
        StaticConfigs.Ports.ToList()
            .ForEach(p => Console.WriteLine($"[DEBUG] Port -> {p}"));
    }

    public static async Task InitializePod(this IServiceProvider sp)
    {
        var serviceProvider = sp.CreateScope().ServiceProvider;
        var podService = serviceProvider.GetRequiredService<IPodService>();
        var self = await podService.InitializePod(StaticConfigs.PodIP, StaticConfigs.Ports);

        if (self != null)
        {
            StaticConfigs.Self = self;
            StaticConfigs.IsInitialized = true;
        }
    }
}
