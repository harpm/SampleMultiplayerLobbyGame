
namespace MultiPlayerLobbyGame.Contracts;

public interface IPodService
{
    Task<bool> InitializePod(string ip, int[] ports);
}
