using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SshController : ControllerBase
{
    private readonly ISshService _sshService;
    private readonly ICredentialsRepository credentialsRepository;

    public SshController(ISshService sshService, ICredentialsRepository credentialsRepository)
    {
        _sshService = sshService;
        this.credentialsRepository = credentialsRepository;
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
    public async Task<IActionResult> AddQuery([FromBody] SshQueryDto queryDto)
    {
        var credentials = await credentialsRepository.GetByIdAndName(queryDto.UserId, queryDto.DatabaseName);

        if (credentials == null)
        {
            return NotFound("Credentials not found");
        }

        var query = new SshQuery
        {
            Query = queryDto.Query,
            QueryName = queryDto.QueryName,
            CredentialId = credentials.Id
        };

        _sshService.AddQuery(query);
        return Ok();
    }
    
    [HttpGet("get-query/{userId}/{databaseName}")]
    public async Task<IActionResult> GetQuery([FromRoute] long userId, [FromRoute] string databaseName)
    {
        var credentials = await credentialsRepository.GetByIdAndName(userId, databaseName);

        if (credentials == null)
        {
            return NotFound("Credentials not found");
        }

        var result = await _sshService.GetQuery(credentials.Id);
        return Ok(result);
    }
    
    [HttpDelete("delete-query/{userId:long}/{databaseName}/{queryName}")]
    public async Task<IActionResult> DeleteQuery([FromRoute] long userId, [FromRoute] string databaseName, [FromRoute] string queryName)
    {
        var credentials = await credentialsRepository.GetByIdAndName(userId, databaseName);

        if (credentials == null)
        {
            return NotFound("Credentials not found");
        }

        _sshService.DeleteQuery(credentials.Id, queryName);
        return Ok();
    }
    
    [HttpPut("update-query")]
    public async Task<IActionResult> UpdateQuery([FromBody] SshQueryDto queryDto)
    {
        var credentials = await credentialsRepository.GetByIdAndName(queryDto.UserId, queryDto.DatabaseName);

        if (credentials == null)
        {
            return NotFound("Credentials not found");
        }

        var query = new SshQuery
        {
            Query = queryDto.Query,
            QueryName = queryDto.QueryName,
            CredentialId = credentials.Id
        };

        var result = await _sshService.UpdateQuery(query);
        return Ok(result);
    }

    [HttpPost("new-connection")]
    public async Task<IActionResult> CreateConnection(AddSshConnectionDbo request)
    {
        var credentials = await credentialsRepository.GetByIdAndName(request.UserId, request.DatabaseName);

        if (credentials == null)
        {
            return NotFound("Credentials not found");
        }

        var connect = new SshConnect
        {
            Ip = request.Ip,
            Port = request.Port,
            Username = request.Username,
            Password = request.Password,
            CredentialId = credentials.Id
        };

        return Ok(await _sshService.CreateSshConnectionOnCredential(connect));
    }

    [HttpGet("all-connections/{userId}")]
    public async Task<IActionResult> GetAllConnections([FromRoute] int userId)
    {
        return Ok(await _sshService.GetAllConnections(userId));
    }
}