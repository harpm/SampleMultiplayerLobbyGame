

using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Share;

public static class StaticConfigs
{
    public static bool IsInitialized = false;
    public static Pod Self = null;
    public static int[] Ports;
    public static string PodIP;
    public static string RedisConnectionString;
}
