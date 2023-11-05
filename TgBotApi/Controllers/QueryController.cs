using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpGet("{userId:long}/{credentialsName}/{queryName}")]
        public async Task<IActionResult> GetQueryByCredentials(long userId, string credentialsName, string queryName)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(queryName) || string.IsNullOrWhiteSpace(credentialsName))
            {
                return BadRequest("Invalid parameters");
            }

            var credentials = await credentialsRepository.GetByIdAndName(userId, credentialsName);

            if (credentials == null)
            {
                return NotFound("Credentials not found");
            }

            var response = await queryRepository.GetByCredentialsAndName(credentials.Id, queryName);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet("{userId:long}/{credentialsName}")]
        public async Task<IActionResult> GetQueriesByCredentials(long userId, string credentialsName)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(credentialsName))
            {
                return BadRequest("Invalid parameters");
            }

            var credentials = await credentialsRepository.GetByIdAndName(userId, credentialsName);

            if (credentials == null)
            {
                return NotFound("Credentials not found");
            }

            var response = await queryRepository.GetByCredentials(credentials.Id);

            return Ok(response);
        }

        [HttpGet("parameters/{userId:long}/{credentialsName}/{queryName}")]
        public async Task<IActionResult> GetQueryParameters(long userId, string credentialsName, string queryName)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(credentialsName) || string.IsNullOrWhiteSpace(queryName))
            {
                return BadRequest("Invalid parameters");
            }

            var credentials = await credentialsRepository.GetByIdAndName(userId, credentialsName);

            if (credentials == null)
            {
                return NotFound("Credentials not found");
            }

            var query = await queryRepository.GetByCredentialsAndName(credentials.Id, queryName);

            if (query == null)
            {
                return NotFound("Query not found");
            }

            var response = await queryParameterRepository.GetByQuery(query.Id);

            return Ok(response);
        }

        [HttpPost("parameters")]
        public async Task<IActionResult> AddQueryParameters([FromBody] QueryParameterDto queryParameterDto)
        {
            if (queryParameterDto.UserId <= 0 || string.IsNullOrWhiteSpace(queryParameterDto.Parameter)
                || string.IsNullOrWhiteSpace(queryParameterDto.DatabaseName)
                || string.IsNullOrWhiteSpace(queryParameterDto.QueryName))
            {
                return BadRequest("Invalid parameter(s)");
            }

            var credentials = await credentialsRepository.GetByIdAndName(queryParameterDto.UserId, queryParameterDto.DatabaseName);

            if (credentials == null)
            {
                return NotFound("Credentials not found");
            }

            var query = await queryRepository.GetByCredentialsAndName(credentials.Id, queryParameterDto.QueryName);

            if (query == null)
            {
                return NotFound("Query not found");
            }

            var queryParameter = new QueryParameter { Parameter = queryParameterDto.Parameter, QueryId = query.Id };

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
            if (queryDto.UserId <= 0 || string.IsNullOrWhiteSpace(queryDto.Sql)
                || string.IsNullOrWhiteSpace(queryDto.DatabaseName) || string.IsNullOrWhiteSpace(queryDto.QueryName))
            {
                return BadRequest("Invalid parameters");
            }

            var credentials = await credentialsRepository.GetByIdAndName(queryDto.UserId, queryDto.DatabaseName);

            if (credentials == null)
            {
                return NotFound("Credentials not found");
            }

            var query = new CustomQuery { CredentialsId = credentials.Id, Sql = queryDto.Sql, Name = queryDto.QueryName };

            var response = await queryRepository.Add(query);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        [HttpPost("execute")]
        public async Task<IActionResult> Execute(ExecuteRequest request)
        {
            var credentials = await credentialsRepository.GetByIdAndName(request.UserId, request.DatabaseName);

            if (credentials == null)
            {
                return NotFound("Credentials not found");
            }

            var result = await queryRepository.Execute(request, credentials);

            return Ok(result);
        }
    }
}
