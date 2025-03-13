using Microsoft.AspNetCore.Mvc;
using MultiPlayerLobbyGame.Contracts.Services;

namespace MultiPlayerLobbyGame.API.Controllers;

[Route("[Controller]")]
public class PlayerController : ControllerBase
{
    protected virtual IPlayerService _service { get; set; }
    public PlayerController(IPlayerService service)
    {
        _service = service;
    }

    [HttpGet("RegisterUser")]
    public Task<Guid> RegisterUser(string name)
    {
        return _service.RegisterPlayer(name);
    }
}
