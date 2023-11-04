using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface IQueryRepository
    {
        Task<CustomQuery?> Get(int Id);
        Task<List<CustomQuery>> GetByCredentials(int credentialsId);
        Task<CustomQuery?> Add(CustomQuery customQuery);
    }
}
