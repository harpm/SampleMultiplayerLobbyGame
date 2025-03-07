using System.Text.Json;
using StackExchange.Redis;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share.Models;
using MultiPlayerLobbyGame.Share;

namespace MultiPlayerLobbyGame.Service.PodServices;

public class PodService : IPodService
{
    protected virtual string _key => "POD";
    protected virtual string _Master_Pod_key => "MASTER_POD";
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

        foreach (var item in podList)
        {
            Console.WriteLine($"[DEBUG]: POD ({item.Id}) -> PORT: {string.Join(", ", item.Ports)}, IP: {item.IP}");
        }

        return podList;
    }

    public virtual async Task<Pod> GetMasterPod()
    {
        Pod result = null;

        try
        {
            var db = _connection.GetDatabase();
            var masterId = await db.StringGetAsync(_Master_Pod_key);

            if (!string.IsNullOrWhiteSpace(masterId))
            {
                var rawMasterPod = await db.HashGetAsync(_key, masterId);
                var masterPod = JsonSerializer.Deserialize<Pod>(rawMasterPod);
                result = masterPod;
            }
        }
        catch (Exception ex)
        {

        }

        return result;
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

            if (!podList.Where(p => p.IsMaster).Any())
            {
                self.IsMaster = true;
                db.StringSetAsync(_Master_Pod_key, self.Id.ToString());
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

    public virtual async Task<bool> MakeMeMaster()
    {
        bool result = false;

        try
        {
            StaticConfigs.Self.IsMaster = true;
            var db = _connection.GetDatabase();
            var masterPodId = await db.StringGetSetAsync(_Master_Pod_key
                , StaticConfigs.Self.Id.ToString());
            var rawOldMasterPod = await db.HashGetAsync(_key, masterPodId);
            if (!string.IsNullOrWhiteSpace(rawOldMasterPod))
            {
                var oldMasterPod = JsonSerializer.Deserialize<Pod>(rawOldMasterPod);
                oldMasterPod.IsMaster = false;
                await db.HashSetAsync(_key
                    , oldMasterPod.Id.ToString()
                    , JsonSerializer.Serialize(oldMasterPod));
            }

            await db.HashSetAsync(_key
                , StaticConfigs.Self.Id.ToString()
                , JsonSerializer.Serialize(StaticConfigs.Self));

            result = true;
        }
        catch (Exception ex)
        {

        }

        return result;
    }
}
