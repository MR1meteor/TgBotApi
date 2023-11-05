using Dapper;
using Newtonsoft.Json;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories
{
    public class QueryRepository : IQueryRepository
    {
        private const string TABLE_NAME = "public.queries";
        private readonly DapperContext context;

        public QueryRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<CustomQuery?> Add(CustomQuery customQuery)
        {
            var query = $@"insert into {TABLE_NAME} (""CredentialsId"", ""Sql"", ""Name"")
                            values (@credentialsId, @sql, @name) returning *";

            var queryArgs = new { CredentialsId = customQuery.CredentialsId, Sql = customQuery.Sql, Name = customQuery.Name };

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<CustomQuery>(query, queryArgs);

                return result?.FirstOrDefault();
            }
        }

        public async Task<ExecuteResponse> Execute(Credentials credentials, string sql)
        {
            try
            {
                var query = sql;

                using (var connection = context.CreateUserConnection(credentials))
                {
                    var result = await connection.QueryAsync(query);
                
                    string serialized = JsonConvert.SerializeObject(result);

                    return new ExecuteResponse(response: serialized);
                }
            }
            catch (Exception ex)
            {
                return new ExecuteResponse(error: ex.Message);
            }
            
        }

        public async Task<CustomQuery?> Get(int id)
        {
            var query = $@"select * from {TABLE_NAME} where ""Id"" = @id";

            var queryArgs = new { Id = id };

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<CustomQuery>(query, queryArgs);

                return result?.FirstOrDefault();
            }
        }

        public async Task<List<CustomQuery>> GetByCredentials(int credentialsId)
        {
            var query = $@"select * from {TABLE_NAME} where ""CredentialsId"" = @credentialsId";

            

            var queryArgs = new { CredentialsId = credentialsId };
            Console.WriteLine($"Типа логи: {query}, {credentialsId}");

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<CustomQuery>(query, queryArgs);

                return result.ToList();
            }
        }

        public async Task<CustomQuery?> GetByCredentialsAndName(int credentialsId, string name)
        {
            var query = $@"select * from {TABLE_NAME} where ""CredentialsId"" = @credentialsId and ""Name"" = @name";

            var queryArgs = new { CredentialsId = credentialsId, Name = name };

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<CustomQuery>(query, queryArgs);

                return result?.FirstOrDefault();
            }
        }
    }
}
