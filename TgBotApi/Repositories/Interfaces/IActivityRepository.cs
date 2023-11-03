using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<StateResponse>> Get(Credentials credentials);
    }
}
