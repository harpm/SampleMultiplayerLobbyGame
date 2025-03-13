namespace MultiPlayerLobbyGame.Contracts.Services;

public interface IPlayerService
{
    Task<Guid> RegisterPlayer(string name);
}
