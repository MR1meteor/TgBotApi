using System.Text;
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

    public async Task<List<SshQuery>> GetQuery(int userId)
    {
        return await _sshRepository.GetQuery(userId);
    }

    public void DeleteQuery(int queryId)
    {
        _sshRepository.DeleteQuery(queryId);
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
                MessageType = "DatabaseDump",
                Object = new DumpModel()
                {
                    UserId = userId,
                    CredentialId = credentials.Id,
                    EventDate = DateTime.Now,
                    SQL = response
                }
            });
            return response;

        }
    }

    public async Task<bool> CreateSshConnectionOnCredential(SshConnect connect)
    {
        return await _sshRepository.SetSshSting(connect);
    }
}