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
            var query = $@"insert into {TABLE_NAME} (""CredentialId"", ""Url"", ""Name"")
                            values (@credentialId, @url, @name)
                            returning *;";

            var queryArgs = new { CredentialId = linkModel.CredentialId, Url = linkModel.Url, Name = linkModel.Name };

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

        public async Task<LinkModel?> GetByCredentialsAndName(int credentialsId, string name)
        {
            var query = $@"select * from {TABLE_NAME} where ""CredentialId"" = @credentialId and ""Name"" = @name";

            var queryArgs = new { CredentialId = credentialsId, Name = name };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<LinkModel>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }
    }
}
