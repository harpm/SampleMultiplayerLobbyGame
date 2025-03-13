using MultiPlayerLobbyGame.Share.Models;
using MultiPlayerLobbyGame.Share;
using MultiPlayerLobbyGame.Contracts.Repositories;
using MultiPlayerLobbyGame.Contracts.Services;

namespace MultiPlayerLobbyGame.Service.PodServices;

public class PodService : IPodService
{
    protected readonly IPodRepository _podRepository;
    protected int _roundRobinCounter = 0;

    public PodService(IPodRepository podRepository)
    {
        _podRepository = podRepository;
    }

    public virtual async Task<Pod> GetMasterPod()
    {
        Pod result = null;

        try
        {
            var masterId = await _podRepository.GetMasterId();

            if (masterId != Guid.Empty)
            {
                result = await _podRepository.GetByIdAsync(masterId);
            }
        }
        catch (Exception ex)
        {

        }

        return result;
    }

    public virtual async Task<Pod> InitializePod(string ip, params int[] ports)
    {
        Pod result = null;

        try
        {
            var self = new Pod()
            {
                Id = Guid.NewGuid(),
                IP = ip,
                Ports = ports,
                IsMaster = false,
            };

            var podList = await _podRepository.GetAllAsync();

            if (!podList.Where(p => p.IsMaster).Any())
            {
                self.IsMaster = true;
                await _podRepository.SetMasterId(self.Id);
            }

            await _podRepository.InsertAsync(self);

            result = self;
        }
        catch (Exception ex)
        {

        }

        return result;
    }

    public virtual async Task<Pod> GetNextPod()
    {
        Pod result = null;

        try
        {
            var podList = await _podRepository.GetAllAsync();
            var next = podList.Skip(_roundRobinCounter)
                .Take(1)
                .First();

            if (++_roundRobinCounter >= podList.Count())
            {
                _roundRobinCounter = 0;
            }

            result = next;
        }
        catch (Exception ex)
        {

        }

        return result;
    }

    public virtual async Task<bool> MakeMeMaster()
    {
        bool result = false;

        try
        {
            await _podRepository.TransactionAsync(async () =>
            {
                var oldMasterPodId = await _podRepository.SwapMasterIdAsync(StaticConfigs.Self.Id);
                var oldMasterPod = await _podRepository.GetByIdAsync(oldMasterPodId);

                oldMasterPod.IsMaster = false;
                await _podRepository.UpdateAsync(oldMasterPodId, oldMasterPod);

                StaticConfigs.Self.IsMaster = true;
                await _podRepository.UpdateAsync(StaticConfigs.Self.Id, StaticConfigs.Self);

                return true;
            });

            result = true;
        }
        catch (Exception ex)
        {

        }

        return result;
    }
}
