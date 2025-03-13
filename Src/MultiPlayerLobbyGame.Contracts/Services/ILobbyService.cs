namespace MultiPlayerLobbyGame.Contracts.Services;

public interface ILobbyService
{
    Task<bool> Connect(Guid playerId);
    Task<bool> Disconnect(Guid playerId);
}
