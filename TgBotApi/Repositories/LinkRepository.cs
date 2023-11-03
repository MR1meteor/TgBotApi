using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private const string TABLE_NAME = "public.links";
        private readonly DapperContext context;

        public LinkRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<LinkModel?> Add(LinkModel linkModel)
        {
            var query = $@"insert into {TABLE_NAME} (""CredentialId"", ""Url"")
                            values (@credentialId, @url)
                            returning *;";

            var queryArgs = new { CredentialId = linkModel.CredentialId, Url = linkModel.Url };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<LinkModel>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }

        public async Task<LinkModel?> Get(int id)
        {
            var query = $@"select * from {TABLE_NAME} where ""Id"" = @id";

            var queryArgs = new { Id = id };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<LinkModel>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }

        public async Task<List<LinkModel>> GetAllByCredential(int credentialId)
        {
            var query = $@"select * from {TABLE_NAME} where ""CredentialId"" = @credentialId";

            var queryArgs = new { CredentialId = credentialId };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<LinkModel>(query, queryArgs);

                return response.ToList();
            }
        }
    }
}
