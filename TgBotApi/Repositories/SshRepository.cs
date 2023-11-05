using System;
using System.Data;
using Dapper;
using Renci.SshNet;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories;

public class SshRepository : ISshRepository
{
    private readonly DapperContext _context;
    private readonly ILogger<SshRepository> _logger;
    private readonly ICredentialsRepository credentialsRepository;
    private const string TABLE_CREDENTIALS = @"public.credentials";
    private const string TABLE_SSH_SERVERS = @"public.ssh_servers";
    private const string TABLE_SSH_QUERYS = @"public.ssh_custom_queries";


    public SshRepository(DapperContext context, ILogger<SshRepository> logger, ICredentialsRepository credentialsRepository)
    {
        this._context = context;
        this._logger = logger;
        this.credentialsRepository = credentialsRepository;
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

    public async Task<SshConnect?> GetConnectByCredentials(int credentialsId)
    {
        var query = $@"select * from {TABLE_SSH_SERVERS} where ""CredentialId"" = @credentialsId";
        var queryArgs = new { CredentialsId = credentialsId };

        using (var connection = _context.CreateDefaultConnection())
        {
            var response = await connection.QueryAsync<SshConnect>(query, queryArgs);

            return response?.FirstOrDefault();
        }
    }


    public async Task<bool> SetSshSting(SshConnect connect)
    {
        try
        {
            var conn = new SshClient(connect.Ip, int.Parse(connect.Port), connect.Username, connect.Password);
            conn.Connect();
            conn.Disconnect();
            
            var query =
                $@"insert into {TABLE_SSH_SERVERS}(""CredentialId"", ""Ip"", ""Port"", ""Username"", ""Password"") values ({connect.CredentialId}, '{connect.Ip}', '{connect.Port}', '{connect.Username}', '{connect.Password}')";

            using (var connection = _context.CreateDefaultConnection())
            {
                await connection.ExecuteAsync(query);
            }
            return true;

        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public void AddQuery(SshQuery query)
    {
        var queryArgs = new
        {
            CredentialId = query.CredentialId,
            Query = query.Query,
            QueryName = query.QueryName,
        };

        var queryInsert = $@"INSERT INTO {TABLE_SSH_QUERYS}(""CredentialId"", ""Query"", ""QueryName"") VALUES(@credentialId, @query, @queryName)";

        using (var connection = _context.CreateDefaultConnection())
        {
            connection.Execute(queryInsert, queryArgs);
        }
    }

    public async Task<List<SshQuery>> GetQuery(int credentialsId)
    {
        var query = $@"select * from {TABLE_SSH_QUERYS} where ""CredentialId"" = @credentialsId";

        var queryArgs = new { CredentialsId = credentialsId };

        using (var connection = _context.CreateDefaultConnection())
        {
            var res = await connection.QueryAsync<SshQuery>(query, queryArgs);
            return res.ToList();
        }

        throw new NotImplementedException();
    }

    public async Task DeleteQuery(int credentialsId, string queryName)
    {
        var query = $@"delete from {TABLE_SSH_QUERYS} where ""CredentialId"" = @credentialId and ""QueryName"" = @queryName";

        var queryArgs = new { CredentialId = credentialsId, QueryName = queryName };

        using (var connection = _context.CreateDefaultConnection())
        {
            await connection.QueryAsync(query, queryArgs);
        }
    }

    public async Task InsertSQLDumps(string sql, int credentialsId)
    {
        using (var connection = _context.CreateDefaultConnection())
        {
            Console.WriteLine(sql);
            var query = $@"insert into Dumps(""sql"", ""credentialsid"") values ('{sql}', {credentialsId})";
            await connection.ExecuteAsync(query);
        }
    }

    public async Task<List<CredentialAndDatabase>> SelectAllConnections(int userId)
    {
        var query =
            @$"select ""CredentialId"", c.""Database"" from ssh_servers s join ""credentials"" ""c"" on s.""CredentialId"" = c.""Id"" where c.""UserId"" = {userId}";
        
            using var connection = _context.CreateDefaultConnection();
            {
                var pair = (await connection.QueryAsync<CredentialAndDatabase>(query)).ToList();
                return pair;
            }
    }

    public async Task<string> SelectDumpSql(int dumpId)
    {
        var query = $@"select ""sql"" from ""dumps"" where ""id""={dumpId} ";

        using var connection = _context.CreateDefaultConnection();
        {
            return (await connection.QueryAsync<string>(query)).FirstOrDefault();
        }
    }

    public async Task<List<DumpModel>> GetDumpsByUserId(int userId, string name)
    {
        var credentials =await  credentialsRepository.GetByIdAndName(userId, name);
        var query = $@"select * from dumps where credentialsid={credentials.Id}";

        using var connection = _context.CreateDefaultConnection();
        {
            var response = await connection.QueryAsync<DumpModel>(query);
            return response.ToList();
        }
    }

    private async Task<int> SelectCredentialsId(int dumpId)
    {
        using var connection = _context.CreateDefaultConnection();
        {
            return await connection.QueryFirstOrDefaultAsync<int>($@"select credentialsid from dumps where id={dumpId};");
        }
    }

    public async Task<Credentials> SelectCredentials(int dumpId)
    {
        var id = await SelectCredentialsId(dumpId);
        return await credentialsRepository.GetById(id);
    }

    public Task<SshQuery> UpdateQuery(SshQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<SshQuery?> GetQueryByCredentials(int credentialsId, string queryName)
    {
        var query = $@"select * from {TABLE_SSH_QUERYS} where ""CredentialId"" = @credentialsId and ""QueryName"" = @queryName";

        var queryArgs = new { CredentialsId = credentialsId, QueryName = queryName };

        using (var connection = _context.CreateDefaultConnection())
        {
            var res = await connection.QueryAsync<SshQuery>(query, queryArgs);
            return res?.FirstOrDefault();
        }
    }

    public async Task<ExecuteResponse> Execute(SshConnect connect, string query)
    {
        try
        {
            var conn = new SshClient(connect.Ip, int.Parse(connect.Port), connect.Username, connect.Password);

            conn.Connect();
            conn.RunCommand(query);
            conn.Disconnect();

            return new ExecuteResponse(response: "Success");
        }
        catch (Exception ex)
        {
            return new ExecuteResponse(error: ex.Message);
        }
    }
}