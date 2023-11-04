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

        public async Task<List<StateChange>> GetErrorStatus(string databaseName)
        {
            var response = new List<StateChange>();
            var query = $@"select state, state_change as StateLastChangeDate, pid, wait_event_type as WaitEventType from pg_stat_activity where datname='{databaseName}';";
            using var connection = context.CreateDefaultConnection();
            {
                List<StateChange> stateChanges = (await connection.QueryAsync<StateChange>(query)).ToList();
                foreach (var stateChange in stateChanges)
                {
                    if (stateChange.WaitEventType == "Lock" && DateTime.Now > stateChange.StateLastChangeDate.AddMinutes(1))
                        response.Add(stateChange);
                }
            }

            return response;
        } 
    }
}
