using Microsoft.AspNetCore.Mvc;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DumpController : ControllerBase
    {
        private readonly ISshService sshService;
        private readonly IMemoryRepository memoryRepository;

        public DumpController(ISshService sshService,
            IMemoryRepository memoryRepository)
        {
            this.sshService = sshService;
            this.memoryRepository = memoryRepository;
        }

        [HttpGet("create/{userId}/{name}")]
        public async Task<IActionResult> CreateDump([FromRoute] int userId, [FromRoute] string name)
        {
            var response = await sshService.CreateDump(userId, name);
            if (string.IsNullOrEmpty(response))
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        
        // TODO add get all dumps by user id 
        // TODO add get all dumps by cred id 

        [HttpGet("load/{userId}/{dumpId}")]
        public async Task<IActionResult> LoadDump([FromRoute] int userId, [FromRoute] int dumpId)
        {
            return (await sshService.LoadDump(dumpId, userId)) ? Ok() : BadRequest();
        }
    }
}