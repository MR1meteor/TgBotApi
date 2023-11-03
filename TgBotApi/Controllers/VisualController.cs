using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisualController : ControllerBase
    {
        private readonly IVisualService visualService;

        public VisualController(IVisualService visualService)
        {
            this.visualService = visualService;
        }

        [HttpGet("{link}")]
        public async Task<IActionResult> GetByLink(string link)
        {
            var encodedScreenshot = await visualService.GetByLink(link);

            if (encodedScreenshot != null)
            {
                return Ok(encodedScreenshot);
            }

            return BadRequest();
        }
    }
}
