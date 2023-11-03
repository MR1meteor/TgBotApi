using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Services
{
    public class LinkService : ILinkService
    {
        private readonly ILinkRepository linkRepository;

        public LinkService(ILinkRepository linkRepository)
        {
            this.linkRepository = linkRepository;
        }

        public async Task<LinkModel?> AddLink(LinkDto model)
        {
            var newLinkModel = new LinkModel
            {
                CredentialId = model.CredentialId,
                Url = model.Url,
            };

            return await linkRepository.Add(newLinkModel);
        }

        public async Task<List<LinkModel>> GetByCredential(int credentialId)
        {
            return await linkRepository.GetAllByCredential(credentialId);
        }

        public async Task<LinkModel?> GetByid(int id)
        {
            return await linkRepository.Get(id);
        }
    }
}
