using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories;

public class SshRepository : ISshRepository
{
    private readonly DapperContext _context;
    private readonly ILogger<SshRepository> _logger;
    private const string TABLE_CREDENTIALS = @"public.credentials";
    private const string TABLE_SSH_SERVERS = @"public.ssh_servers";
    private const string TABLE_SSH_QUERYS = @"public.ssh_custom_queries";


    public SshRepository(DapperContext context, ILogger<SshRepository> logger)
    {
        this._context = context;
        this._logger = logger;
    }

    public async Task<SshConnect> GetSshString(int userId)
    {
        ;
        var query =
            $@"select s.""Ip"" as Ip, s.""Port"" as Port, s.""Password"" as Password, s.""Username"" as Username  from {TABLE_CREDENTIALS} c JOIN {TABLE_SSH_SERVERS} s ON c.""Id"" = s.""CredentialId"" where c.""UserId"" = @userid ";

        var queryArgs = new { UserId = userId };

        using (var connection = _context.CreateDefaultConnection())
        {
            var response = await connection.QueryAsync<SshConnect>(query, queryArgs);

            return response.FirstOrDefault();
        }
    }

    public void AddQuery(SshQuery query)
    {
        var queryArgs = new
        {
            CredentialId = query.UserId,
            Query = query.Query,
            QueryName = query.QueryName,
        };

        var queryInsert = $@"INSERT INTO {TABLE_SSH_QUERYS}(""CredentialId"", ""Query"", ""QueryName"") VALUES(@CredentialId, @Query, @QueryName)";

        using (var connection = _context.CreateDefaultConnection())
        {
            connection.Execute(queryInsert, queryArgs);
        }
    }

    public async Task<List<SshQuery>> GetQuery(int userId)
    {
        var query = $@"select * from {TABLE_SSH_QUERYS} where ""CredentialId"" = @userId";

        var queryArgs = new { UserId = userId };

        using (var connection = _context.CreateDefaultConnection())
        {
            var res = await connection.QueryAsync<SshQuery>(query, queryArgs);
            return res.ToList();
        }

        throw new NotImplementedException();
    }

    public void DeleteQuery(int queryId)
    {
        throw new NotImplementedException();
    }

    public Task<SshQuery> UpdateQuery(SshQuery query)
    {
        throw new NotImplementedException();
    }
}