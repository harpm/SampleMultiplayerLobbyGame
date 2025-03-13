
using MultiPlayerLobbyGame.Contracts;

namespace MultiPlayerLobbyGame.Share.Models;

public class Player : RedisModel<Guid>
{
    public string Name { get; set; }
    public Guid JoinedLobby { get; set; }
}
