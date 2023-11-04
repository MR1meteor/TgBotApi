using Dapper;
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
            var query = $@"insert into {TABLE_NAME} (""CredentialsId"", ""Sql"")
                            values (@credentialsId, @sql) returning *";

            var queryArgs = new { CredentialsId = customQuery.CredentialsId, Sql = customQuery.Sql };

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<CustomQuery>(query, queryArgs);

                return result?.FirstOrDefault();
            }
        }

        //public async Task Execute(ExecuteRequest executeRequest)
        //{
        //    var query = executeRequest.Sql;
        //    var queryArgs = { nameof(executeRequest.Sql), executeRequest.Sql };

        //    using (var connection = context.CreateDefaultConnection())
        //    {
        //        var result = await connection.QueryAsync(query, queryArgs);

        //        return;
        //    }
        //}

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

            using (var connection = context.CreateDefaultConnection())
            {
                var result = await connection.QueryAsync<CustomQuery>(query, queryArgs);

                return result.ToList();
            }
        }
    }
}
