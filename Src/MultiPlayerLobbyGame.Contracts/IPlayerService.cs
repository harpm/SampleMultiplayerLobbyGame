
namespace MultiPlayerLobbyGame.Contracts;

public interface IPlayerService
{
    Task<Guid> RegisterPlayer();
}
