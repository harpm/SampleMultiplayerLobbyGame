
using System.Text.Json;
using StackExchange.Redis;
using MultiPlayerLobbyGame.Contracts;
using MultiPlayerLobbyGame.Share.Models;
using MultiPlayerLobbyGame.Service.PlayerServices;

namespace MultiPlayerLobbyGame.Service.LobbyServices;

public class LobbyService : ILobbyService
{
    protected virtual string _key => "LOBBY";
    protected virtual int _maxPlayerCount => 64;
    protected readonly IConnectionMultiplexer _connection;

    public LobbyService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connection = connectionMultiplexer;
    }

    public virtual async Task<bool> Connect(Guid playerId)
    {
        bool result = false;

        try
        {
            var db = _connection.GetDatabase();

            // Check if player ID is valid
            var rawPlayer = await db.HashGetAsync(PlayerService._key, playerId.ToString());
            if (!rawPlayer.HasValue || rawPlayer.IsNull || string.IsNullOrWhiteSpace(rawPlayer))
            {
                throw new ArgumentException("The requested player does not exist." +
                    " Please register first and try again with a valid player ID.");
            }

            var player = JsonSerializer.Deserialize<Player>(rawPlayer);

            // Check if the player has already joined a lobby
            if (player.JoinedLobby != Guid.Empty)
            {
                throw new ArgumentException("Requested player is already connected to a lobby.");
            }


            // Check if any free lobby exists and creates new one if not
            var rawLobbyList = await db.HashGetAllAsync(_key);
            var lobbyList = rawLobbyList.Select(p => JsonSerializer.Deserialize<Lobby>(p.Value));
            var freeLobby = lobbyList.Where(l => l.PlayersCount < _maxPlayerCount).FirstOrDefault();
            if (freeLobby == null)
            {
                freeLobby = new Lobby()
                {
                    Id = Guid.NewGuid(),
                    PlayersCount = 0
                };

                await db.HashSetAsync(_key, freeLobby.Id.ToString(), JsonSerializer.Serialize(freeLobby));
            }

            player.JoinedLobby = freeLobby.Id;

            // Increase players count in lobby
            freeLobby.PlayersCount++;
            await db.HashSetAsync(_key, freeLobby.Id.ToString(), JsonSerializer.Serialize(freeLobby));

            await db.HashSetAsync(PlayerService._key, player.Id.ToString(), JsonSerializer.Serialize(player));

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
            var db = _connection.GetDatabase();

            // Check if player ID is valid
            var rawPlayer = await db.HashGetAsync(PlayerService._key, playerId.ToString());
            if (!rawPlayer.HasValue || rawPlayer.IsNull || string.IsNullOrWhiteSpace(rawPlayer))
            {
                throw new ArgumentException("The requested player does not exist." +
                    " Please register first and try again with a valid player ID.");
            }

            var player = JsonSerializer.Deserialize<Player>(rawPlayer);

            
            if (player.JoinedLobby == Guid.Empty)
            {
                throw new ArgumentException("Requested player has not connected to any lobby yet.");
            }

            // Check if any free lobby exists and creates new one if not
            var rawLobbyList = await db.HashGetAllAsync(_key);
            var lobbyList = rawLobbyList.Select(l => JsonSerializer.Deserialize<Lobby>(l.Value));
            var joinedLobby = lobbyList.Where(l => l.Id == player.JoinedLobby).FirstOrDefault();

            if (joinedLobby == null)
            {
                throw new InvalidDataException("The joined lobby does not exist.");
            }

            // Increase players count in lobby
            joinedLobby.PlayersCount--;
            await db.HashSetAsync(_key, joinedLobby.Id.ToString(), JsonSerializer.Serialize(joinedLobby));

            // Finally remove the player lobby id from the Database
            player.JoinedLobby = Guid.Empty;
            await db.HashSetAsync(PlayerService._key
                , player.Id.ToString()
                , JsonSerializer.Serialize(player));

            result = true;
        }
        catch (Exception ex)
        {
            result = false;
        }

        return result;
    }
}
