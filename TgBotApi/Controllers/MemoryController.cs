using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemoryController : ControllerBase
    {
        private readonly ICredentialsRepository credentialsRepository;
        private readonly IMemoryRepository memoryRepository;
        private readonly ILogger<VacuumController> logger;

        public MemoryController(ICredentialsRepository credentialsRepository,
            IMemoryRepository memoryRepository)
        {
            this.credentialsRepository = credentialsRepository;
            this.memoryRepository = memoryRepository;
        }

        [HttpGet("memory/{userId}/{name}")]
        public async Task<IActionResult> GetMemoryCorrelation([FromRoute] int userId, [FromRoute] string name)
        {
            return Ok(await memoryRepository.GetMemoryCorrelations(userId, name));
        }

        [HttpPost("memory/{userId}/{name}")]
        public async Task<IActionResult> UpdateMemoryCorrelations([FromRoute] int userId, [FromRoute] string name, [FromBody] MemoryCorrelations req)
        {
            var res = await memoryRepository.UpdateMemoryCorrelations(req, userId, name);
            if (!res)
            {
                return BadRequest();
            }
            
            return Ok();
        }
    }
}