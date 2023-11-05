using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface IQueryRepository
    {
        Task<CustomQuery?> Get(int Id);
        Task<CustomQuery?> GetByCredentialsAndName(int credentialsId, string name);
        Task<List<CustomQuery>> GetByCredentials(int credentialsId);
        Task<CustomQuery?> Add(CustomQuery customQuery);
        Task<string?> Execute(ExecuteRequest executeRequest, Credentials credentials);
    }
}
