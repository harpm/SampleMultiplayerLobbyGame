using MultiPlayerLobbyGame.Contracts.Services;
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
        var httpClient = context.RequestServices
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient();

        if (!StaticConfigs.Self.IsMaster)
        {

            if (context.Request.Headers.ContainsKey(IsFromMasterHeader))
            {
                await _next(context);
            }
            else
            {
                // Check if master is healthy
                var masterPod = await podService.GetMasterPod();
                if (!await CheckPodHealth(masterPod, httpClient))
                {
                    await podService.MakeMeMaster();
                }

                await SendRequestToPod(context, masterPod);
            }
        }
        else
        {
            var isHealthy = false;
            do
            {
                Pod nextPod = await podService.GetNextPod();

                if (nextPod.Id == StaticConfigs.Self.Id)
                {
                    await _next(context);
                    break;
                }
                else if (nextPod.Id != Guid.Empty)
                {
                    isHealthy = await CheckPodHealth(nextPod, httpClient);
                    if (!isHealthy)
                    {
                        continue;
                    }

                    await SendRequestToPod(context, nextPod);
                }
            } while (!isHealthy);
        }
    }

    private async Task SendRequestToPod(HttpContext context, Pod nextPod)
    {
        var httpClient = context.RequestServices.GetRequiredService<HttpClient>();
        httpClient.BaseAddress = new Uri($"{nextPod.IP}:{nextPod.Ports.First()}");
        var method = HttpMethod.Parse(context.Request.Method);
        var req = new HttpRequestMessage(method, context.Request.Path.ToString());
        req.Headers.Add(IsFromMasterHeader, true.ToString());
        var result = await httpClient.SendAsync(req);
        context.Response.StatusCode = (int)result.StatusCode;
        foreach (var item in result.Headers)
        {
            context.Response.Headers.Append(item.Key, item.Value.ToString());
        }

        await context.Response.WriteAsync(await result.Content.ReadAsStringAsync());
    }

    private async Task<bool> CheckPodHealth(Pod nextPod, HttpClient httpClient)
    {
        bool isHealthy = false;
        httpClient.BaseAddress = new Uri($"{nextPod.IP}:{nextPod.Ports.First()}");
        var res = await httpClient.GetAsync("/healthy");
        if (res.StatusCode == System.Net.HttpStatusCode.OK)
        {
            isHealthy = true;
        }
        return isHealthy;
    }
}
