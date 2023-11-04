using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly IQueryRepository queryRepository;
        private readonly IQueryParameterRepository queryParameterRepository;
        private readonly ICredentialsRepository credentialsRepository;

        public QueryController(IQueryRepository queryRepository, IQueryParameterRepository queryParameterRepository, ICredentialsRepository credentialsRepository)
        {
            this.queryRepository = queryRepository;
            this.queryParameterRepository = queryParameterRepository;
            this.credentialsRepository = credentialsRepository;
        }

        [HttpGet("{credentialsId:int}")]
        public async Task<IActionResult> GetQueriesByCredentials([FromRoute] int credentialsId)
        {
            if (credentialsId <= 0)
            {
                return BadRequest("Invalid 'CredentialsId'");
            }

            var response = await queryRepository.GetByCredentials(credentialsId);

            return Ok(response);
        }

        [HttpGet("parameters/{queryId:int}")]
        public async Task<IActionResult> GetQueryParameters([FromRoute] int queryId)
        {
            if (queryId <= 0)
            {
                return BadRequest("Invalid 'QueryId'");
            }

            var response = await queryParameterRepository.GetByQuery(queryId);

            return Ok(response);
        }

        [HttpPost("parameters")]
        public async Task<IActionResult> AddQueryParameters([FromBody] QueryParameterDto queryParameterDto)
        {
            if (queryParameterDto.QueryId <= 0 || string.IsNullOrWhiteSpace(queryParameterDto.Parameter))
            {
                return BadRequest("Invalid parameter(s)");
            }

            var query = await queryRepository.Get(queryParameterDto.QueryId);

            if (query == null)
            {
                return NotFound("Query not found");
            }

            var queryParameter = new QueryParameter { Parameter = queryParameterDto.Parameter, QueryId = queryParameterDto.QueryId };
            
            var response = await queryParameterRepository.AddQueryParameter(queryParameter);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuery([FromBody] CustomQueryDto queryDto)
        {
            if (queryDto.CredentialsId <= 0 || string.IsNullOrWhiteSpace(queryDto.Sql))
            {
                return BadRequest("Invalid parameters");
            }

            var credentials = await credentialsRepository.GetById(queryDto.CredentialsId);

            if (credentials == null)
            {
                return NotFound("Credentials not found");
            }

            var query = new CustomQuery { CredentialsId = queryDto.CredentialsId, Sql = queryDto.Sql };

            var response = await queryRepository.Add(query);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        //[HttpPost("execute")]
        //public async Task<IActionResult> Execute(ExecuteRequest request)
        //{
        //    await queryRepository.Execute(request);

        //    return Ok();
        //}
    }
}
