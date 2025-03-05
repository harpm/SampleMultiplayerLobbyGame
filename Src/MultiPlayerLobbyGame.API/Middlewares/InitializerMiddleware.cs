using System.Net;
using System.Net.Sockets;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share;

namespace MultiPlayerLobbyGame.API.Middlewares;

public class InitializerMiddleware
{
    private readonly RequestDelegate _next;

    public InitializerMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public virtual async Task InvokeAsync(HttpContext context)
    {
        if (!StaticConfigs.IsInitialized)
        {
            var entry = Dns.GetHostEntry(Dns.GetHostName());
            StaticConfigs.PodIP = entry.AddressList
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.ToString())
                .FirstOrDefault();

            Console.WriteLine($"[DEBUG]: HostName -> {StaticConfigs.PodIP}");

            StaticConfigs.Ports = [context.Request.Host.Port ?? 0];
            Console.WriteLine($"[DEBUG]: Port -> {StaticConfigs.PodIP}");


            var serviceProvider = context.RequestServices;
            var podService = serviceProvider.GetRequiredService<IPodService>();
            var self = await podService.InitializePod(StaticConfigs.PodIP, StaticConfigs.Ports);

            if (self != null)
            {
                StaticConfigs.Self = self;
                StaticConfigs.IsInitialized = true;
            }
        }

        if (StaticConfigs.IsInitialized)
        {
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Initialization failed...");
        }
    }
}
