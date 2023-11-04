using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/credentials")]
    [ApiController]
    public class MetricController : ControllerBase
    {
        private readonly ICredentialsRepository credentialsRepository;
        private readonly IMetricRepository metricRepository;

        public MetricController(ICredentialsRepository credentialsRepository, IMetricRepository metricRepository)
        {
            this.credentialsRepository = credentialsRepository;
            this.metricRepository = metricRepository;
        }

        [HttpGet("stat-database/{userId}/{name}")]
        public async Task<IActionResult> GetStatDatabaseMetric([FromRoute] int userId, [FromRoute] string name)
        {
            return Ok(await metricRepository.GetStatDatabaseMetric(userId, name));
        }
    }
}