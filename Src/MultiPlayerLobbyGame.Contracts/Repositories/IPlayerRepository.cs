using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Contracts.Repositories;

public interface IPlayerRepository : IRedisRepository<Player, Guid>
{
}
