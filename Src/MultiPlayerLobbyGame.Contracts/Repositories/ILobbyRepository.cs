using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Contracts.Repositories;

public interface ILobbyRepository : IRedisRepository<Lobby, Guid>
{
}
