
using MultiPlayerLobbyGame.Contracts.Services;
using MultiPlayerLobbyGame.Share.Models;
using MultiPlayerLobbyGame.Data;

namespace MultiPlayerLobbyGame.Service.LobbyServices;

public class LobbyService : ILobbyService
{
    protected virtual int _maxPlayerCount => 64;
    protected readonly LobbyRepository lobbyRepository;
    protected readonly PlayerRepository playerRepository;

    public LobbyService()
    {
    }

    public virtual async Task<bool> Connect(Guid playerId)
    {
        bool result = false;

        try
        {
            // Check if player ID is valid
            var player = await playerRepository.GetByIdAsync(playerId);

            // Check if the player has already joined a lobby
            if (player.JoinedLobby != Guid.Empty)
            {
                throw new ArgumentException("Requested player is already connected to a lobby.");
            }

            // Check if any free lobby exists and creates new one if not
            var lobbyList = await lobbyRepository.GetAllAsync();
            var freeLobby = lobbyList.Where(l => l.PlayersCount < _maxPlayerCount).FirstOrDefault();
            if (freeLobby == null)
            {
                freeLobby = new Lobby()
                {
                    Id = Guid.NewGuid(),
                    PlayersCount = 0
                };

                await lobbyRepository.InsertAsync(freeLobby);
            }

            player.JoinedLobby = freeLobby.Id;

            // Increase players count in lobby
            freeLobby.PlayersCount++;
            await lobbyRepository.UpdateAsync(freeLobby.Id, freeLobby);

            await playerRepository.UpdateAsync(playerId, player);

            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }

        return result;
    }

    public virtual async Task<bool> Disconnect(Guid playerId)
    {
        bool result = false;

        try
        {
            var player = await playerRepository.GetByIdAsync(playerId);
            
            if (player.JoinedLobby == Guid.Empty)
            {
                throw new ArgumentException("Requested player has not connected to any lobby yet.");
            }

            // Check if any free lobby exists and creates new one if not
            var lobbyList = await lobbyRepository.GetAllAsync();
            var joinedLobby = lobbyList.Where(l => l.Id == player.JoinedLobby).FirstOrDefault();

            if (joinedLobby == null)
            {
                throw new InvalidDataException("The joined lobby does not exist.");
            }

            // Increase players count in lobby
            joinedLobby.PlayersCount--;
            await lobbyRepository.UpdateAsync(joinedLobby.Id, joinedLobby);

            // Finally remove the player lobby id from the Database
            player.JoinedLobby = Guid.Empty;
            await playerRepository.UpdateAsync(playerId, player);

            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }

        return result;
    }
}
