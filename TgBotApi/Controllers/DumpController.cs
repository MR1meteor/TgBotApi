using Microsoft.AspNetCore.Mvc;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DumpController : ControllerBase
    {
        private readonly ICredentialsRepository credentialsRepository;
        private readonly IMemoryRepository memoryRepository;

        public DumpController(ICredentialsRepository credentialsRepository,
            IMemoryRepository memoryRepository)
        {
            this.credentialsRepository = credentialsRepository;
            this.memoryRepository = memoryRepository;
        }

        [HttpGet("dump/{userId}/{name}")]
        public async Task<IActionResult> CreateDump([FromRoute] int userId, [FromRoute] string name)
        {
            var response = await memoryRepository.CreateDatabaseDump(userId, name);
            if (string.IsNullOrEmpty(response))
            {
                return StatusCode(500, response);
            }
            return Ok(response);
        }
    }
}