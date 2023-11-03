using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private const string TABLE_NAME = @"""pg_catalog"".""pg_stat_activity""";
        private readonly DapperContext context;
        
        public ActivityRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<List<StateResponse>> Get(Credentials credentials)
        {
            var query = $@"select ""datname"", ""state"" from {TABLE_NAME} where ""datname"" is not null";

            using (var connection = context.CreateUserConnection(credentials))
            {
                var states = await connection.QueryAsync<StateResponse>(query);
                return states.ToList();
            }
        }
    }
}
