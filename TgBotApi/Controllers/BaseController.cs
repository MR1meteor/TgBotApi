using Microsoft.AspNetCore.Mvc;

namespace TgBotApi.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok("Pong suka, zaebali");
        }
    }
}
