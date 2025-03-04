using MultiPlayerLobbyGame.Share;
using MultiPlayerLobbyGame.Share.Utills;

namespace MultiPlayerLobbyGame.API;

internal static class StartUp
{
    public static void InitializePod(this WebApplicationBuilder builder)
    {
        StaticConfigs.Ports = builder.GetHostPorts();
        StaticConfigs.PodIP = System.Net.Dns.GetHostName();

        // TODO: add pod info to the shared memmory
    }

    public static int[] GetHostPorts(this WebApplicationBuilder builder)
    {
        return builder.Host.Properties.Where(p => (p.Key as Type).Name == "WebHostOptions")
            .SelectMany(p => StringUtils.ExtractPorts((string)p.Value.GetType().GetProperty("ServerUrls").GetValue(p.Value)))
            .ToArray();
    }
}
