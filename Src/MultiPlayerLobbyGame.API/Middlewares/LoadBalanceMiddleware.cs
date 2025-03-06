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
            var podService = context.RequestServices.GetRequiredService<IPodService>();
            var nextPod = await podService.GetNextPod();

            if (nextPod.Id == StaticConfigs.Self.Id)
            {
                await _next(context);
            }
            else
            {
                var httpClient = context.RequestServices.GetRequiredService<HttpClient>();
                httpClient.BaseAddress = new Uri($"{nextPod.IP}:{nextPod.Ports.First()}");
                var method = HttpMethod.Parse(context.Request.Method);
                var req = new HttpRequestMessage(method, context.Request.Path.ToString());
                var result = await httpClient.SendAsync(req);
                context.Response.StatusCode = (int) result.StatusCode;
                foreach (var item in result.Headers)
                {
                    context.Response.Headers.Append(item.Key, item.Value.ToString());
                }
                await context.Response.WriteAsync(await result.Content.ReadAsStringAsync());
            }
            
        }
    }
}
