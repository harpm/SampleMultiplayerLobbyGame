
using StackExchange.Redis;
using MultiPlayerLobbyGame.Share.Models;
using MultiPlayerLobbyGame.Contracts.Repositories;

namespace MultiPlayerLobbyGame.Data;

public class PodRepository : RedisRepositoryBase<Pod, Guid>, IPodRepository
{
    protected virtual string _Master_Pod_key => "MASTER_POD";
    protected override string _key => "POD";
    public PodRepository(IConnectionMultiplexer connectionMultiplexer)
        : base(connectionMultiplexer)
    {

    }

    public async Task<Guid> GetMasterId()
    {
        Guid result = Guid.Empty;

        string rawMasterId = await _database.StringGetAsync(_Master_Pod_key);

        if (!string.IsNullOrWhiteSpace(rawMasterId))
        {
            Guid.TryParse(rawMasterId, out result);
        }

        return result;
    }

    public async Task<bool> SetMasterId(Guid masterId)
    {
        bool result = false;

        if (masterId == Guid.Empty) throw new ArgumentNullException(nameof(masterId));

        result = await _database.StringSetAsync(_Master_Pod_key, masterId.ToString());

        return result;
    }

    public async Task<Guid> SwapMasterIdAsync(Guid id)
    {
        Guid result = Guid.Empty;

        if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));

        var rawOldId = await _database.StringGetSetAsync(_key, id.ToString());

        if (!string.IsNullOrWhiteSpace(rawOldId))
        {
            Guid.TryParse(rawOldId, out result);
        }

        return result;
    }
}
