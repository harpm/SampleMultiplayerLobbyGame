
using MultiPlayerLobbyGame.Contracts.Repositories;
using MultiPlayerLobbyGame.Share.Models;
using StackExchange.Redis;

namespace MultiPlayerLobbyGame.Data;

public class LobbyRepository : RedisRepositoryBase<Lobby, Guid>, ILobbyRepository
{
    protected override string _key => "LOBBY";
    public LobbyRepository(IConnectionMultiplexer connectionMultiplexer)
        : base(connectionMultiplexer)
    {

    }
}
