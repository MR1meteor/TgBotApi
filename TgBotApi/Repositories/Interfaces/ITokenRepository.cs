using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<Token?> GetByUser(long userId);
        Task<Token?> Add(Token token);
        Task<Token?> GetByToken(string token);
        Task<Token?> Update(Token token);
    }
}
