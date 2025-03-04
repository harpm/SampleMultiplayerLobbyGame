
namespace MultiPlayerLobbyGame.Contracts;

public interface IPodService
{
    Task<bool> InitializePod(int[] ports, string ip);
}
