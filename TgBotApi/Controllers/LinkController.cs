using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
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

        [HttpGet("id/{userId:long}/{databaseName}/{linkName}")]
        public async Task<IActionResult> GetLinkById([FromRoute] long userId, string databaseName, string linkName)
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

            var result = await linkService.GetByCredentialsAndName(credential.Id, linkName);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound("Link not found");
        }

        [HttpGet("credential/{userId:long}/{databaseName}")]
        public async Task<IActionResult> GetLinkByCredential([FromRoute] long userId, string databaseName)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(databaseName))
            {
                return BadRequest("Invalid parameters");
            }

            var credential = await credentialsRepository.GetByIdAndName(userId, databaseName);

            if (credential == null)
            {
                return NotFound("Credentials not found");
            }

            var links = await linkService.GetByCredential(credential.Id);

            return Ok(links);
        }

        [HttpPost]
        public async Task<IActionResult> AddLink(LinkDto request)
        { 
            if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.DatabaseName)
                || string.IsNullOrWhiteSpace(request.Url) || string.IsNullOrWhiteSpace(request.LinkName))
            {
                return BadRequest("Invalid parameters");
            }
            
            var validLink = linkService.ValidateLink(request.Url);

            if (!validLink)
            {
                return BadRequest("Invalid link. Please, write it in format: 'https://...' or 'http://...'");
            }

            var credential = await credentialsRepository.GetByIdAndName(request.UserId, request.DatabaseName);

            if (credential == null)
            {
                return NotFound("Credentials not found");
            }

            var response = await linkService.AddLink(credential.Id, request.Url, request.LinkName);

            if (response != null)
            {
                return Ok(response);
            }

            return BadRequest();
        }
    }
}
