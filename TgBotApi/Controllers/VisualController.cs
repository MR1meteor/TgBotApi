using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Repositories;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisualController : ControllerBase
    {
        private readonly IVisualService visualService;
        private readonly ICredentialsRepository credentialsRepository;
        private readonly ILinkService linkService;

        public VisualController(IVisualService visualService, ICredentialsRepository credentialsRepository, ILinkService linkService)
        {
            this.visualService = visualService;
            this.credentialsRepository = credentialsRepository;
            this.linkService = linkService;
        }

        //[HttpGet("{link}")]
        //public async Task<IActionResult> GetByLink(string link)
        //{
        //    var encodedScreenshot = visualService.GetByLink(link);

        //    if (encodedScreenshot != null)
        //    {
        //        return Ok(encodedScreenshot);
        //    }

        //    return BadRequest();
        //}

        [HttpGet("{userId:long}/{databaseName}/{linkName}")]
        public async Task<IActionResult> GetByCredentials([FromRoute] long userId, string databaseName, string linkName)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(linkName))
            {
                return BadRequest("Invalid parameters");
            }

            var credential = await credentialsRepository.GetByIdAndName(userId, databaseName);

            if (credential == null)
            {
                return NotFound("Credentials not found");
            }

            var link = await linkService.GetByCredentialsAndName(credential.Id, linkName);

            if (link == null)
            {
                return NotFound("Link not found");
            }

            var encodedScreenshot = await visualService.GetByLink(link.Url);

            if (encodedScreenshot != null)
            {
                return Ok(encodedScreenshot);
            }

            return BadRequest();
        }
    }
}
