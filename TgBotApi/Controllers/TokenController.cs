using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenRepository tokenRepository;

        public TokenController(ITokenRepository tokenRepository)
        {
            this.tokenRepository = tokenRepository;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> ValidateToken(string token)
        {
            var existingToken = await tokenRepository.GetByToken(token);

            if (existingToken == null)
            {
                return NotFound("Token not found");
            }

            if (existingToken.LastLogin.AddMinutes(30) > DateTime.UtcNow)
            {
                return BadRequest("Token expired");
            }

            return Ok("Token valid");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(TokenDto token)
        {
            var existingToken = await tokenRepository.GetByUser(token.UserId);

            if (existingToken != null)
            {
                return Conflict("Token already exists, update token instead");
            }

            var newToken = new Token
            {
                UserId = token.UserId,
                TokenValue = token.TokenValue,
                LastLogin = DateTime.UtcNow,
            };

            var response = await tokenRepository.Add(newToken);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> RefreshToken(TokenDto token)
        {
            var existingToken = await tokenRepository.GetByUser(token.UserId);

            if (existingToken == null)
            {
                return NotFound("Token not found, create token instead");
            }

            var updatedToken = new Token
            {
                Id = existingToken.Id,
                TokenValue = token.TokenValue,
                UserId = token.UserId,
                LastLogin = DateTime.UtcNow
            };

            var response = await tokenRepository.Update(updatedToken);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }
    }
}
