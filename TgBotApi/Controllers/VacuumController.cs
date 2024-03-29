﻿using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacuumController : ControllerBase
    {
        private readonly IVacuumRepository vacuumRepository;
        private readonly ICredentialsRepository credentialsRepository;
        private readonly ILogger<VacuumController> logger;

        public VacuumController(IVacuumRepository activityRepository, ICredentialsRepository credentialsRepository,
            ILogger<VacuumController> logger)
        {
            this.vacuumRepository = activityRepository;
            this.credentialsRepository = credentialsRepository;
            this.logger = logger;
        }

        [HttpGet("full/{userId}/{name}")]
        public async Task<IActionResult> VacuumFullRefresh([FromRoute] int userId, [FromRoute] string name)
        {
            var credentials = await credentialsRepository.GetByIdAndName(userId, name);
            var res = await vacuumRepository.VacuumFull(credentials);
            return Ok(res);
        }
    }
}