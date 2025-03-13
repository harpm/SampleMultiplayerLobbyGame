
using StackExchange.Redis;
using MultiPlayerLobbyGame.Share.Models;
using MultiPlayerLobbyGame.Contracts.Repositories;

namespace MultiPlayerLobbyGame.Data;

public class PlayerRepository : RedisRepositoryBase<Player, Guid>, IPlayerRepository
{
    protected override string _key => "PLAYER";
    public PlayerRepository(IConnectionMultiplexer connectionMultiplexer)
        : base(connectionMultiplexer)
    {

    }
}
