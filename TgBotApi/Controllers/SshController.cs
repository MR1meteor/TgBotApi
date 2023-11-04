using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SshController : ControllerBase
{
    private readonly ISshService _sshService;

    public SshController(ISshService sshService)
    {
        _sshService = sshService;
    }
    
    [HttpGet("check-disk-space/{userId}")]
    public async Task<IActionResult> CheckDiskSpace([FromRoute] int userId)
    {
        var result = await _sshService.CheckDiskSpace(userId);
        return Ok(result);
    }
    
    [HttpGet("lsof/{userId}")]
    public async Task<IActionResult> Lsof([FromRoute] int userId)
    {
        var result = await _sshService.Lsof(userId);
        return Ok(result);
    }
    
    [HttpGet("tcpdump/{userId}")]
    public async Task<IActionResult> Tcpdump([FromRoute] int userId)
    {
        var result = await _sshService.Tcpdump(userId);
        return Ok(result);
    }
    
    [HttpPost("add-query")]
    public async Task<IActionResult> AddQuery([FromBody] SshQuery query)
    {
        _sshService.AddQuery(query);
        return Ok();
    }
    
    [HttpGet("get-query/{userId}")]
    public async Task<IActionResult> GetQuery([FromRoute] int userId)
    {
        var result = await _sshService.GetQuery(userId);
        return Ok(result);
    }
    
    [HttpDelete("delete-query/{queryId}")]
    public async Task<IActionResult> DeleteQuery([FromRoute] int queryId)
    {
        _sshService.DeleteQuery(queryId);
        return Ok();
    }
    
    [HttpPut("update-query")]
    public async Task<IActionResult> UpdateQuery([FromBody] SshQuery query)
    {
        var result = await _sshService.UpdateQuery(query);
        return Ok(result);
    }

    [HttpPost("new-connection")]
    public async Task<IActionResult> CreateConnection(SshConnect connect)
    {
        return Ok(await _sshService.CreateSshConnectionOnCredential(connect));
    }
}