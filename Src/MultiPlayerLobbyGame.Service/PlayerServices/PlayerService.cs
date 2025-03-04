
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace MultiPlayerLobbyGame.Service.PlayerServices;

public class PlayerService : IPlayerService
{
    public static string _key => "PLAYER";
    protected readonly IConnectionMultiplexer _connection;

    public PlayerService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connection = connectionMultiplexer;
    }

    public virtual async Task<Guid> RegisterPlayer(string name)
    {
        Guid result = Guid.Empty;

        try
        {
            var newPlayer = new Player()
            {
                Id = Guid.NewGuid(),
                Name = name,
                JoinedLobby = Guid.Empty
            };

            var db = _connection.GetDatabase();
            await db.HashSetAsync(_key, newPlayer.Id.ToString(), JsonSerializer.Serialize(newPlayer));
            result = newPlayer.Id;
        }
        catch (Exception ex)
        {
            result = Guid.Empty;
        }

        return result;
    }
}
