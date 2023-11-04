using System.Text;
using Renci.SshNet;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Services;

public class SshService : ISshService
{
    private readonly ISshRepository _sshRepository;

    public SshService(ISshRepository sshRepository)
    {
        this._sshRepository = sshRepository;
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
      return  await _sshRepository.GetQuery(userId);
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
}