using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface ICredentialsRepository
    {
        Task<bool> Add(Credentials creds);
        Task<Credentials?> Get(string dbname, long userId);
        Task<List<Credentials>> GetByUser(long userId);
    }
}
