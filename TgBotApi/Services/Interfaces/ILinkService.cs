using TgBotApi.Models;

namespace TgBotApi.Services.Interfaces
{
    public interface ILinkService
    {
        Task<LinkModel?> AddLink(int credentialId, string url, string name);
        Task<LinkModel?> GetByid(int id);
        Task<List<LinkModel>> GetByCredential(int credentialId);
        Task<LinkModel?> GetByCredentialsAndName(int credentialsId, string name);
        bool ValidateLink(string link);
    }
}
