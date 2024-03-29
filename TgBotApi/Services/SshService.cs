﻿using System.Text;
using KafkaClient.Interfaces;
using KafkaClient.Models;
using Renci.SshNet;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Services;

public class SshService : ISshService
{
    private readonly ISshRepository _sshRepository;
    private readonly ICredentialsRepository credentialsRepository;
    private readonly IKafkaProducesService kafkaProducesService;

    public SshService(ISshRepository sshRepository, ICredentialsRepository credentialsRepository, IKafkaProducesService kafkaProducesService)
    {
        this._sshRepository = sshRepository;
        this.credentialsRepository = credentialsRepository;
        this.kafkaProducesService = kafkaProducesService;
    }

    public async Task<string> CheckDiskSpace(int UserId)
    {
        var cred = await _sshRepository.GetSshString(UserId);
        return await ExecuteCommand($"df -h", CreateConnection(cred));
    }

    public async Task<string> Lsof(int UserId)
    {
        var cred = await _sshRepository.GetSshString(UserId);
        return await ExecuteCommand($"Lsof", CreateConnection(cred));
    }

    public async Task<string> Tcpdump(int UserId)
    {
        var cred = await _sshRepository.GetSshString(UserId);
        return await ExecuteCommand($"tcpdump -i enp0s3", CreateConnection(cred));
    }

    public void AddQuery(SshQuery query)
    {
        _sshRepository.AddQuery(query);
    }

    public async Task<List<SshQuery>> GetQuery(int credentialsId)
    {
        return await _sshRepository.GetQuery(credentialsId);
    }

    public void DeleteQuery(int credentialsId, string queryName)
    {
        _sshRepository.DeleteQuery(credentialsId, queryName);
    }

    public async Task<SshQuery> UpdateQuery(SshQuery query)
    {
        return await _sshRepository.UpdateQuery(query);
    }

    private SshClient CreateConnection(SshConnect cred)
    {
        return new SshClient(cred.Ip, int.Parse(cred.Port), cred.Username, cred.Password);
    }

    private Task<string> ExecuteCommand(string command, SshClient client)
    {
        client.Connect();
        var stringBuilder = new StringBuilder();
        var cmd = client.CreateCommand(command);
        var result = cmd.BeginExecute();
        using (var reader = new StreamReader(cmd.OutputStream, Encoding.UTF8, true, 1024, true))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line != null)
                {
                    stringBuilder.Append(line + "\n");
                }
            }

            cmd.EndExecute(result);
        }

        client.Disconnect();
        return Task.FromResult(stringBuilder.ToString());
    }

    private Task ExecuteCommandWithOutOutput(string command, SshClient client)
    {
        client.Connect();
        var cmd = client.CreateCommand(command);
        var result = cmd.BeginExecute();
        cmd.EndExecute(result);
        client.Disconnect();
        return Task.FromResult("");
    }

    public async Task<string> CreateDump(int userId, string name)
    {
        var credentials = await credentialsRepository.GetByIdAndName(userId, name);
        var cred = await _sshRepository.GetSshString(userId);
        var pgDumpCommand = $"pg_dump -U {credentials.Database} > test";
        var catCommand = $"cat test";

        using (var connection = CreateConnection(cred))
        {
            await ExecuteCommandWithOutOutput(pgDumpCommand, connection);
            var response = await ExecuteCommand(catCommand, connection);
            await kafkaProducesService.WriteTraceLogAsync(new Message()
            {
                MessageType = string.IsNullOrEmpty(response) ? "FailedDatabaseDump" : "SuccessDatabaseDump",
                Object = new DumpModel()
                {
                    UserId = userId,
                    CredentialId = credentials.Id,
                    EventDate = DateTime.Now,
                    SQL = response
                }
            });
            if (!string.IsNullOrEmpty(response))
                await _sshRepository.InsertSQLDumps(response.Replace('\'', '"'), credentials.Id);
            return response;

        }
    }

    public async Task<bool> LoadDump(int dumpId, int userId)
    {
        var connectionDbCred = await _sshRepository.SelectCredentials(dumpId); 
        var cred = await _sshRepository.GetSshString(userId);
        var pgDumpCommand = $"pg_dump -U {connectionDbCred.Database} < dump.sql";
        var sql = await _sshRepository.SelectDumpSql(dumpId);

        await File.WriteAllTextAsync("./dump.sql", sql.Replace('"', '\''));

        using var connection = CreateConnection(cred) ;
        try
        {
            await ExecuteCommandWithOutOutput(pgDumpCommand, connection);
            await kafkaProducesService.WriteTraceLogAsync(new Message()
            {
                MessageType = "SuccessDatabaseDumpLoad",
                Object = new DumpModel()
                {
                    UserId = userId,
                    CredentialId = connectionDbCred.Id,
                    EventDate = DateTime.Now,
                    SQL = sql
                }
            });
            return true;
        }
        catch (Exception ex)
        {
            await kafkaProducesService.WriteTraceLogAsync(new Message()
            {
                MessageType = "FailDatabaseDumpLoad",
                Object = new DumpModel()
                {
                    UserId = userId,
                    CredentialId = connectionDbCred.Id,
                    EventDate = DateTime.Now,
                    SQL = sql
                }
            });
            return false;
        }
    }

    public async Task<List<CredentialAndDatabase>> GetAllConnections(int userId)
    {
        return await _sshRepository.SelectAllConnections(userId);
    }

    public async Task<bool> CreateSshConnectionOnCredential(SshConnect connect)
    {
        return await _sshRepository.SetSshSting(connect);
    }

    public async Task<List<DumpModel>> GetDumpsByUserId(int userId, string name)
    {
        return await _sshRepository.GetDumpsByUserId(userId, name);
    }

    public async Task<SshConnect?> GetSshConnection(int credentialsId)
    {
        return await _sshRepository.GetConnectByCredentials(credentialsId);
    }

    public async Task<ExecuteResponse> Execute(SshConnect connect, string query)
    {
        return await _sshRepository.Execute(connect, query);
    }

    public async Task<SshQuery?> GetQueryByCreds(int credentialsId, string queryName)
    {
        return await _sshRepository.GetQueryByCredentials(credentialsId, queryName);
    }
}