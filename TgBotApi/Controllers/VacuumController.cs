using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacuumController : ControllerBase
    {
        private readonly IVacuumRepository vacuumRepository;
        private readonly ICredentialsRepository credentialsRepository;
        private readonly ILogger<VacuumController> logger;

        public VacuumController(IVacuumRepository activityRepository, ICredentialsRepository credentialsRepository,
            ILogger<VacuumController> logger)
        {
            this.vacuumRepository = activityRepository;
            this.credentialsRepository = credentialsRepository;
            this.logger = logger;
        }

        [HttpPost("full/{userId}/{name}")]
        public async Task<IActionResult> VacuumFullRefresh([FromRoute] int userId, [FromRoute] string name, [FromBody] Credentials cr)
        {
            // var credentials = await credentialsRepository.GetByIdAndName(userId, name);
            // logger.LogDebug(credentials.ToString());
            var res = await vacuumRepository.VacuumFull(cr);
            return Ok(res);
        }
    }
}