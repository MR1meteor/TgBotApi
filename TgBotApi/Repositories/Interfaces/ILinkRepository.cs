using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface ILinkRepository
    {
        Task<LinkModel?> Add(LinkModel linkModel);
        Task<LinkModel?> Get(int id);
        Task<List<LinkModel>> GetAllByCredential(int credentialId);
    }
}
