using System.Text.RegularExpressions;
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

        public async Task<LinkModel?> AddLink(int credentialId, string url, string name)
        {
            var validLink = System.Web.HttpUtility.UrlDecode(url);

            var newLinkModel = new LinkModel
            {
                CredentialId = credentialId,
                Url = validLink,
                Name = name
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

        public async Task<LinkModel?> GetByCredentialsAndName(int credentialsId, string name)
        {
            return await linkRepository.GetByCredentialsAndName(credentialsId, name);
        }

        public bool ValidateLink(string link)
        {
            var url = System.Web.HttpUtility.UrlDecode(link);

            var pattern = "^[https://|http://]";
            var regex = new Regex(pattern);

            var match = regex.Match(url);

            return match.Success;
        }
    }
}
