using TgBotApi.Models;

namespace TgBotApi.Services.Interfaces
{
    public interface ILinkService
    {
        Task<LinkModel?> AddLink(LinkDto model);
        Task<LinkModel?> GetByid(int id);
        Task<List<LinkModel>> GetByCredential(int credentialId);
    }
}
