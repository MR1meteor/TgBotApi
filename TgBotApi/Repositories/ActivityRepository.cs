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
        private readonly ICredentialsRepository credentialsRepository;
        
        public ActivityRepository(DapperContext context, ICredentialsRepository credentialsRepository)
        {
            this.context = context;
            this.credentialsRepository = credentialsRepository;
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

        public async Task<List<StateChange>> GetErrorStatus(Credentials credentials)
        {
            var response = new List<StateChange>();
            var query = $@"select state, state_change as StateLastChangeDate, pid, wait_event_type as WaitEventType from pg_stat_activity where datname='{credentials?.Database}';";
            using var connection = context.CreateUserConnection(credentials);
            {
                List<StateChange> stateChanges = (await connection.QueryAsync<StateChange>(query)).ToList();
                foreach (var stateChange in stateChanges)
                {
                    if (stateChange.WaitEventType == "Lock" && DateTime.Now > stateChange.StateLastChangeDate.AddMinutes(1))
                        response.Add(stateChange);
                }
            }

            for (int i = 0; i < response.Count; i++)
            {
                response[i].userId = credentials.UserId;
                response[i].DataBase = credentials.Database;
            }

            return response;
        }

        public async Task KillTransaction(Credentials credentials)
        {
                using var tr = context.CreateUserConnection(credentials);
                {
                    var response = await GetErrorStatus(credentials);
                    foreach (var st in response)
                    {
                        var query = $@"SELECT pg_terminate_backend({st.Pid});";
                        await tr.ExecuteAsync(query);
                    }
                }
        }
        
        public async Task<List<StateChange>> GetAllErrorStatus()
        {
            var response = new List<StateChange>();
            var query = $@"select state, state_change as StateLastChangeDate, pid, wait_event_type as WaitEventType from pg_stat_activity";
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
