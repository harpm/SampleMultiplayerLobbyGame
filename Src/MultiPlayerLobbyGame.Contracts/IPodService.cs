
using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Contracts;

public interface IPodService
{
    Task<Pod> InitializePod(string ip, int[] ports);
    Task<Pod> GetMasterPod();
    Task<Pod> GetNextPod();
}
