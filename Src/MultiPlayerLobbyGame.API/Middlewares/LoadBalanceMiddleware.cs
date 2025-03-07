using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share;
using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.API.Middlewares;

public class LoadBalanceMiddleware
{
    private const string IsFromMasterHeader = "FromMaster";
    private readonly RequestDelegate _next;

    public LoadBalanceMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public virtual async Task InvokeAsync(HttpContext context)
    {
        var podService = context.RequestServices.GetRequiredService<IPodService>();
        if (!StaticConfigs.Self.IsMaster)
        {
            if (context.Request.Headers.ContainsKey(IsFromMasterHeader))
            {
                await _next(context);
            }
            else
            {
                var masterPod = await podService.GetMasterPod();
                await SendRequestToPod(context, masterPod);
            }
        }
        else
        {
            var nextPod = await podService.GetNextPod();

            if (nextPod.Id == StaticConfigs.Self.Id)
            {
                await _next(context);
            }
            else
            {
                await SendRequestToPod(context, nextPod);
            }
        }
    }

    public async Task SendRequestToPod(HttpContext context, Pod nextPod)
    {
        var httpClient = context.RequestServices.GetRequiredService<HttpClient>();
        httpClient.BaseAddress = new Uri($"{nextPod.IP}:{nextPod.Ports.First()}");
        var method = HttpMethod.Parse(context.Request.Method);
        var req = new HttpRequestMessage(method, context.Request.Path.ToString());
        var result = await httpClient.SendAsync(req);
        context.Response.StatusCode = (int)result.StatusCode;
        foreach (var item in result.Headers)
        {
            context.Response.Headers.Append(item.Key, item.Value.ToString());
        }
        await context.Response.WriteAsync(await result.Content.ReadAsStringAsync());
    }
}
