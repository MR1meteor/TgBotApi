using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/credentials")]
    [ApiController]
    public class CredentialsController : ControllerBase
    {
        private readonly ICredentialsRepository credentialsRepository;

        public CredentialsController(ICredentialsRepository credentialsRepository)
        {
            this.credentialsRepository = credentialsRepository;
        }

        [HttpGet("{userId:long}")]
        public async Task<IActionResult> GetCredentials(long userId)
        {
            var result = await credentialsRepository.GetByUser(userId);
            if (result.Error != null)
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCredentials(CredentialsDto credentials)
        {
            var creds = new Credentials
            {
                Name = credentials.Name,
                UserId = credentials.UserId,
                Host = credentials.Host,
                Port = credentials.Port,
                Database = credentials.Database,
                Username = credentials.Username,
                Password = credentials.Password,
            };

            var result = await credentialsRepository.Add(creds);

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
