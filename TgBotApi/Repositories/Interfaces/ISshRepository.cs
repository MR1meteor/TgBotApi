using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces;

public interface ISshRepository
{
    Task<SshConnect> GetSshString(int UserId);
    Task<bool> SetSshSting(SshConnect connect);
    void AddQuery(SshQuery query);
    Task<List<SshQuery>> GetQuery(int credentialsId);
    Task DeleteQuery(int credentialsId, string queryName);
    Task InsertSQLDumps(string sql, int credentialsId); 
    Task<SshQuery> UpdateQuery(SshQuery query);
    Task<List<CredentialAndDatabase>> SelectAllConnections(int userId);
    Task<string> SelectDumpSql(int dumpId);
    Task<Credentials> SelectCredentials(int dumpId);
}