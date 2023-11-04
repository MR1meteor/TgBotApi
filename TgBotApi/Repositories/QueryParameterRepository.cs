using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories
{
    public class QueryParameterRepository : IQueryParameterRepository
    {
        private const string TABLE_NAME = "public.query_parameters";
        private readonly DapperContext context;

        public QueryParameterRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<QueryParameter?> AddQueryParameter(QueryParameter queryParameter)
        {
            var query = $@"insert into {TABLE_NAME} (""QueryId"", ""Parameter"")
                            values (@queryId, @parameter) returning *";

            var queryArgs = new { QueryId = queryParameter.QueryId, Parameter = queryParameter.Parameter };

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<QueryParameter>(query, queryArgs);

                return result?.FirstOrDefault();
            }
        }

        public async Task<List<QueryParameter>> GetByQuery(int queryId)
        {
            var query = $@"select * from {TABLE_NAME} where ""QueryId"" = @queryId";

            var queryArgs = new { QueryId = queryId };

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<QueryParameter>(query, queryArgs);

                return result.ToList();
            }
        }
    }
}
