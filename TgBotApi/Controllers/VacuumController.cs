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

        public VacuumController(IVacuumRepository activityRepository, ICredentialsRepository credentialsRepository)
        {
            this.vacuumRepository = activityRepository;
            this.credentialsRepository = credentialsRepository;
        }

        [HttpGet("full/{userId}/{name}")]
        public async Task<IActionResult> VacuumFullRefresh([FromRoute] int userId, [FromRoute] string name)
        {
            var crdes = await credentialsRepository.GetByIdAndName(userId, name);

            var res = await vacuumRepository.VacuumFull(crdes);
            return Ok(res);
        }
        
    }
}