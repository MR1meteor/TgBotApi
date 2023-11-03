using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly ILinkService linkService;
        private readonly ICredentialsRepository credentialsRepository;

        public LinkController(ILinkService linkService, ICredentialsRepository credentialsRepository)
        {
            this.linkService = linkService;
            this.credentialsRepository = credentialsRepository;
        }

        [HttpGet("id/{id:int}")]
        public async Task<IActionResult> GetLinkById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid 'Id'");
            }

            var result = await linkService.GetByid(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound("Link not found");
        }

        [HttpGet("credential/{credentialId:int}")]
        public async Task<IActionResult> GetLinkByCredential([FromRoute] int credentialId)
        {
            if (credentialId <= 0)
            {
                return BadRequest("Invalid 'CredentialId'");
            }

            var credential = await credentialsRepository.GetById(credentialId);

            if (credential == null)
            {
                return NotFound("Credentials not found");
            }

            var links = await linkService.GetByCredential(credentialId);

            return Ok(links);
        }

        [HttpPost]
        public async Task<IActionResult> AddLink(LinkDto request)
        {
            var response = await linkService.AddLink(request);

            if (response != null)
            {
                return Ok(response);
            }

            return BadRequest();
        }
    }
}
