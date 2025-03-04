using System.Text.Json;
using StackExchange.Redis;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Service.PodServices;

public class PodService : IPodService
{
    protected virtual string _key => "POD";
    protected readonly IConnectionMultiplexer _connection;

    public PodService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connection = connectionMultiplexer;
    }

    public virtual async Task<bool> InitializePod(string ip, params int[] ports)
    {
        bool result = false;

        try
        {
            var self = new Pod()
            {
                Id = new Guid(),
                IP = ip,
                Ports = ports,
                IsMaster = false,
            };

            var db = _connection.GetDatabase();
            var rawPodList = await db.HashGetAllAsync(_key);
            var podList = rawPodList.Select(p => JsonSerializer.Deserialize<Pod>(p.Value));
            if (podList.Where(p => p.IsMaster).Any())
            {
                self.IsMaster = true;
            }
            await db.HashSetAsync(_key
                , self.Id.ToString()
                , JsonSerializer.Serialize(self));

            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }

        return result;
    }
}
