
namespace MultiPlayerLobbyGame.Share.Models;

public class Pod
{
    public Guid Id { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
    public bool IsMaster { get; set; }
}
