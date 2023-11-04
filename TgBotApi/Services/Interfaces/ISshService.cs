using TgBotApi.Models;

namespace TgBotApi.Services.Interfaces;

public interface ISshService
{
    Task<string> CheckDiskSpace(int userId);
    Task<string> Lsof(int userId);
    Task<string> Tcpdump(int userId);
    
    void AddQuery(SshQuery query);
    Task<List<SshQuery>> GetQuery(int userId);
    void DeleteQuery(int queryId);
    Task<SshQuery> UpdateQuery(SshQuery query);

}