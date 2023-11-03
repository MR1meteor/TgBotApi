using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface ICredentialsRepository
    {
        Task<bool> Add(Credentials creds);
        Task<Credentials?> Get(string dbname, long userId);
        Task<AllCredentials> GetByUser(long userId);
        Task<AllCredentials> GetAllCredentials();
    }
}
