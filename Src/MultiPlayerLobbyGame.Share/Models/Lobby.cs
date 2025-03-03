
namespace MultiPlayerLobbyGame.Share.Models;

public class Lobby
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public byte PlayersCount { get; set; }
}
