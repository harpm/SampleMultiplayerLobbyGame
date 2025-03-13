
using MultiPlayerLobbyGame.Contracts;

namespace MultiPlayerLobbyGame.Share.Models;

public class Pod : RedisModel<Guid>
{
    public string IP { get; set; }
    public int[] Ports { get; set; }
    public bool IsMaster { get; set; }
}
