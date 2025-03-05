using StackExchange.Redis;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Service;
using MultiPlayerLobbyGame.Share;

namespace MultiPlayerLobbyGame.API;

internal static class StartUp
{
    public static void InjectRequiredServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
            ConnectionMultiplexer.Connect(StaticConfigs.RedisConnectionString));

        builder.Services.AddMPLServices();

        builder.Services.AddSingleton<HttpClient>(new HttpClient());
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
    }
}
