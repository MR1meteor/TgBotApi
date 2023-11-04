using System.Drawing.Printing;
using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces;

public class MetricRepository : IMetricRepository
{
    private const string TABLE_NAME = @"public.credentials";
    private readonly DapperContext context;
    private readonly ILogger<MetricRepository> logger;
    private readonly ICredentialsRepository credentialsRepository;

    public MetricRepository(DapperContext context, ILogger<MetricRepository> logger, ICredentialsRepository credentialsRepository)
    {
        this.context = context;
        this.logger = logger;
        this.credentialsRepository = credentialsRepository;
    }

    public async Task<StatDatabase> GetStatDatabaseMetric(int userId, string name)
    {
        var credentials = await credentialsRepository.GetByIdAndName(userId, name);
        
        using var connection = context.CreateUserConnection(credentials);
        {
            var query = @$"select sum(xact_commit + xact_rollback) as transactionCount,
                            sum(tup_fetched) as getCount,
                            sum(tup_inserted) as insertCount,
                            sum(tup_updated) as updateCount,
                            sum(tup_deleted) as deleteCount,
                            sum(tup_returned) as returnCount,
                            sum(conflicts) as conflictCount,
                            sum(deadlocks) as deadlockSum,
                            max(session_time) as sessionTime,
                            sum(sessions) as sesssionCount,
                            sum(sessions_killed) as sessionKilledCount,
                            sum(sessions_fatal) as sessionAbandonedCount
                        from pg_stat_database where datname = '{credentials.Database}';";
            return await connection.QueryFirstOrDefaultAsync<StatDatabase>(query);
        }
    }

    public async Task<string?> GetDatabaseMemory(Credentials? credentials)
    {
        var query = @$"SELECT pg_size_pretty(pg_database_size('{credentials?.Database}')) table_size";
        string response;
        
        using var connection = context.CreateUserConnection(credentials);
        {
            response = await connection.QueryFirstOrDefaultAsync<string>(query);
        }
        connection.Close();

        return response;

    }
}