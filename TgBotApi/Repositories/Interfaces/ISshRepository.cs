using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces;

public interface ISshRepository
{
    Task<SshConnect> GetSshString(int UserId);
    Task<bool> SetSshSting(SshConnect connect);
    void AddQuery(SshQuery query);
    Task<List<SshQuery>> GetQuery(int userId);
    void DeleteQuery(int queryId);

    Task<SshQuery> UpdateQuery(SshQuery query);
}