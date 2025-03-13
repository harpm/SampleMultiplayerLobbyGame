using MultiPlayerLobbyGame.Share.Models;

namespace MultiPlayerLobbyGame.Contracts.Services;

public interface IPodService
{
    Task<Pod> InitializePod(string ip, int[] ports);
    Task<Pod> GetMasterPod();
    Task<Pod> GetNextPod();
    Task<bool> MakeMeMaster();
}
