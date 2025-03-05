using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share;

namespace MultiPlayerLobbyGame.API.Middlewares;

public class LoadBalanceMiddleware
{
    private readonly RequestDelegate _next;

    public LoadBalanceMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public virtual async Task InvokeAsync(HttpContext context)
    {
        if (!StaticConfigs.Self.IsMaster)
        {
            await _next(context);
        }
        else
        {
            // Just for now
            await _next(context);
            return;
            var podService = context.RequestServices.GetRequiredService<IPodService>();
            var nextPod = await podService.GetNextPod();
            
            var httpClient = context.RequestServices.GetRequiredService<HttpClient>();
            httpClient.BaseAddress = new Uri($"{nextPod.IP}:{nextPod.Ports.First()}");
            var method = HttpMethod.Parse(context.Request.Method);
            var req = new HttpRequestMessage(method, context.Request.Path.ToString());
            // TODO: Send the request to the next pod instance
            
        }
    }
}
