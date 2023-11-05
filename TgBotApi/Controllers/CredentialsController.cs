using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
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

        [HttpPost("by-string")]
        public async Task<IActionResult> AddCredentialsByString(CredentialsStringDto request)
        {
            var pattern = "^Server=[\\w]+;Port=[\\d]+;Database=[\\w]+;Uid=[\\w]+;Pwd=[\\w]+"; // Server=IP address;Port=5432;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
            var regex = new Regex(pattern);
            var match = regex.Match(request.ConnectionString);

            if (!match.Success)
            {
                return BadRequest();
            }

            var credsPairs = request.ConnectionString.Split(';');
            var credsDict = new Dictionary<string, string>();

            var credentials = new Credentials
            {
                Name = request.Name,
                UserId = request.UserId,
                Host = credsPairs[0].Split('=')[1],
                Port = credsPairs[1].Split('=')[1],
                Database = credsPairs[2].Split('=')[1],
                Username = credsPairs[3].Split('=')[1],
                Password = credsPairs[4].Split('=')[1]
            };

            return Ok(await credentialsRepository.Add(credentials));
        }
    }
}
