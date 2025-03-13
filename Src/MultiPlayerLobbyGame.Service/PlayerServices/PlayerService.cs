
using MultiPlayerLobbyGame.Contracts.Repositories;
using MultiPlayerLobbyGame.Contracts.Services;
using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Service.PlayerServices;

public class PlayerService : IPlayerService
{
    protected readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        this._playerRepository = playerRepository;
    }

    public virtual async Task<Guid> RegisterPlayer(string name)
    {
        Guid result = Guid.Empty;

        try
        {
            var newPlayer = new Player()
            {
                Id = Guid.NewGuid(),
                Name = name,
                JoinedLobby = Guid.Empty
            };

            await _playerRepository.InsertAsync(newPlayer);
            result = newPlayer.Id;
        }
        catch (Exception ex)
        {
            result = Guid.Empty;
        }

        return result;
    }
}
