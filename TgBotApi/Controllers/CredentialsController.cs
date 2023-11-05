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
            var regex = new Regex("Server=([A-Za-z0-9]+(\\.[A-Za-z0-9]+)+);Port=[A-Za-z0-9]+;Database=[A-Za-z0-9]+;Uid=[A-Za-z0-9]+;Pwd=[A-Za-z0-9]+;", RegexOptions.IgnoreCase);
            var match = regex.IsMatch(request.ConnectionString);

            if (!match)
            {
                return BadRequest("Invalid connection string");
            }

            var credsPairs = request.ConnectionString.Split(';');

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

            Console.WriteLine($"{credentials.Host}, {credentials.Port}, {credentials.Database}, {credentials.Username}, {credentials.Password}");

            return Ok(await credentialsRepository.Add(credentials));
        }
    }
}
