
using MultiPlayerLobbyGame.Contracts;

namespace MultiPlayerLobbyGame.Share.Models;

public class Lobby : RedisModel<Guid>
{
    public byte PlayersCount { get; set; }
}
