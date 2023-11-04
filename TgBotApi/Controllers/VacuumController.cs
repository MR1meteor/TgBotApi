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

        public VacuumController(IVacuumRepository activityRepository)
        {
            this.vacuumRepository = activityRepository;
        }

        [HttpPost("full")]
        public async Task<IActionResult> VacuumFullRefresh([FromBody] Credentials req)
        {
            var res = await vacuumRepository.VacuumFull(req);
            return Ok(res);
        }
        
    }
}