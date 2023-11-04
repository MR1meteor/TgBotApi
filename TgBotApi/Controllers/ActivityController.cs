using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityRepository activityRepository;
        private readonly ICredentialsRepository credentialsRepository;

        public ActivityController(IActivityRepository activityRepository, ICredentialsRepository credentialsRepository)
        {
            this.activityRepository = activityRepository;
            this.credentialsRepository = credentialsRepository;
        }

        [HttpPost]
        public async Task<IActionResult> GetState([FromBody] StateDto request)
        {
            var creds = await credentialsRepository.Get(request.DbName, request.UserId);

            if (creds == null)
            {
                return NotFound();
            }

            var states = await activityRepository.Get(creds);
            return Ok(states);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStates()
        {
            return Ok();
        }

        [HttpGet("get-error-stats/{databaseName}")]
        public async Task<IActionResult> GetErrorStatus([FromRoute] string databaseName)
        {
            return Ok(await activityRepository.GetErrorStatus(databaseName));
        }
    }
}
