using System.Text.Json;
using StackExchange.Redis;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Service.PodServices;

public class PodService : IPodService
{
    protected virtual string _key => "POD";
    protected readonly IConnectionMultiplexer _connection;
    protected int _roundRobinCounter = 0;

    public PodService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connection = connectionMultiplexer;
    }

    protected virtual async Task<IEnumerable<Pod>> InternalGetAll(IDatabase db)
    {
        var rawPodList = await db.HashGetAllAsync(_key);
        var podList = rawPodList.Select(p => JsonSerializer.Deserialize<Pod>(p.Value));
        return podList.ToList();
    }

    public virtual async Task<Pod> InitializePod(string ip, params int[] ports)
    {
        Pod result = null;

        try
        {
            var self = new Pod()
            {
                Id = Guid.NewGuid(),
                IP = ip,
                Ports = ports,
                IsMaster = false,
            };

            var db = _connection.GetDatabase();
            var podList = await InternalGetAll(db);

            if (podList.Where(p => p.IsMaster).Any())
            {
                self.IsMaster = true;
            }
            await db.HashSetAsync(_key
                , self.Id.ToString()
                , JsonSerializer.Serialize(self));

            result = self;
        }
        catch (Exception ex)
        {
            
        }

        return result;
    }

    public virtual async Task<Pod> GetNextPod()
    {
        Pod result = null;

        try
        {
            var db = _connection.GetDatabase();
            var podList = await InternalGetAll(db);
            var next = podList.Skip(_roundRobinCounter)
                .Take(1)
                .First();

            if (++_roundRobinCounter >= podList.Count())
            {
                _roundRobinCounter = 0;
            }

            result = next;
        }
        catch (Exception ex)
        {

        }

        return result;
    }
}
