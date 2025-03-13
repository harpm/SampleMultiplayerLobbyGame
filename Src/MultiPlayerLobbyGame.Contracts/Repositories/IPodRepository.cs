using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Contracts.Repositories;

public interface IPodRepository : IRedisRepository<Pod, Guid>
{
    Task<Guid> GetMasterId();
    Task<bool> SetMasterId(Guid masterId);
    Task<Guid> SwapMasterIdAsync(Guid id);
}
