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

        [HttpGet("dump/{userId}/{name}")]
        public async Task<IActionResult> CreateDump([FromRoute] int userId, [FromRoute] string name)
        {
            var response = await sshService.CreateDump(userId, name);
            return Ok(response);
        }
    }
}