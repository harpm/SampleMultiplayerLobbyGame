
namespace MultiPlayerLobbyGame.Share.Models;

public class Player
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid JoinedLobby { get; set; }
}
