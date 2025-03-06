using Microsoft.AspNetCore.Mvc;
using MultiPlayerLobbyGame.Contracts;

namespace MultiPlayerLobbyGame.API.Controllers;

[Route("[Controller]")]
public class LobbyController
{
    protected virtual ILobbyService _service { get; set; }

    public LobbyController(ILobbyService service)
    {
        _service = service;
    }

    [HttpGet("Connect")]
    public Task Connect(Guid playerId)
    {
        return _service.Connect(playerId);
    }

    [HttpGet("Disconnect")]
    public Task Disconnect(Guid playerId)
    {
        return _service.Disconnect(playerId);
    }
}
