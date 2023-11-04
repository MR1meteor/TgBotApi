using KafkaClient.Interfaces;
using KafkaClient.Models;
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
        private readonly IKafkaProducesService kafkaProduces;

        public ActivityController(IActivityRepository activityRepository, ICredentialsRepository credentialsRepository, IKafkaProducesService kafkaProduces)
        {
            this.activityRepository = activityRepository;
            this.credentialsRepository = credentialsRepository;
            this.kafkaProduces = kafkaProduces;
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
        
        [HttpGet("get-error-stats/{userId}/{name}")]
        public async Task<IActionResult> GetErrorStatusByUserIdAndName([FromRoute] long userId, [FromRoute] string name)
        {
            var creds = await credentialsRepository.GetByUser(userId);
            var res = await activityRepository.GetErrorStatus(creds?.CredentialsList?[0]);
            if (res.Count > 0)
            {
                var message = new Message();
                message.MessageType = "LockStatus";
                message.Object = res;
                await kafkaProduces.WriteTraceLogAsync(message);
            }

            return Ok(res);
        }

        [HttpGet("kill-transaction/{userId}/{database}")]
        public async Task<IActionResult> KillTransaction([FromRoute] long userId, [FromRoute] string database)
        {
            var credentials = await credentialsRepository.GetByDatabaseAndUserId(userId, database);
            await activityRepository.KillTransaction(credentials);
            return Ok();
        }
    }
}
