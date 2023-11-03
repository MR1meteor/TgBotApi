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

        [HttpGet("stat-database/{datname}")]
        public async Task<IActionResult> GetStatDatabaseMetric([FromRoute] string datname)
        {
            return Ok(await metricRepository.GetStatDatabaseMetric(datname));
        }
    }
}