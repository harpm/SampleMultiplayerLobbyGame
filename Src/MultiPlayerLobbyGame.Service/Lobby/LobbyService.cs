
using MultiPlayerLobbyGame.Contracts;

namespace MultiPlayerLobbyGame.Service.Lobby;

public class LobbyService : ILobbyService
{
    public Task<bool> Connect(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Disconnect(Guid playerId)
    {
        throw new NotImplementedException();
    }
}
