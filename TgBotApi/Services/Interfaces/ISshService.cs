using TgBotApi.Models;

namespace TgBotApi.Services.Interfaces;

public interface ISshService
{
    Task<string> CheckDiskSpace(int UserId);
    Task<string> Lsof(int UserId);
    Task<string> Tcpdump(int UserId);
    
    void AddQuery(SshQuery query);
    Task<List<SshQuery>> GetQuery(int userId);
    void DeleteQuery(int queryId);
    Task<SshQuery> UpdateQuery(SshQuery query);

}