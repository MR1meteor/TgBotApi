using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface IQueryParameterRepository
    {
        Task<List<QueryParameter>> GetByQuery(int queryId);
        Task<QueryParameter?> AddQueryParameter(QueryParameter queryParameter);
    }
}
