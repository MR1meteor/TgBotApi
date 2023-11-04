using Dapper;
using System.Data;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories
{
    public class CredentialsRepository : ICredentialsRepository
    {
        private const string TABLE_NAME = @"public.credentials";
        private readonly DapperContext context;
        private readonly ILogger<CredentialsRepository> logger;

        public CredentialsRepository(DapperContext context, ILogger<CredentialsRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<bool> Add(Credentials creds)
        {
            var query =
                $@"insert into {TABLE_NAME}(""{nameof(Credentials.Name)}"", ""{nameof(Credentials.UserId)}"", ""{nameof(Credentials.Host)}"",
                            ""{nameof(Credentials.Port)}"", ""{nameof(Credentials.Database)}"", ""{nameof(Credentials.Username)}"", ""{nameof(Credentials.Password)}"")
                            values (@name, @userId, @host, @port, @database, @username, @password)
                            returning *";

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Credentials>(query, creds);

                if (response.FirstOrDefault() != null)
                {
                    return true;
                }

                return false;
            }
        }

        public async Task<Credentials?> Get(string name, long userId)
        {
            var query =
                $@"select * from {TABLE_NAME} where ""{nameof(Credentials.Name)}"" = @name and ""{nameof(Credentials.UserId)}"" = @userId";

            var queryArgs = new { Name = name, UserId = userId };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Credentials>(query, queryArgs);

                return response.FirstOrDefault();
            }
        }

        public async Task<AllCredentials> GetByUser(long userId)
        {
            var query = $@"select * from credentials where ""{nameof(Credentials.UserId)}"" = @userId";
            var allCredentials = new AllCredentials();

            var queryArgs = new { UserId = userId };

            using var connection = context.CreateDefaultConnection();
            try
            {
                var response = await connection.QueryAsync<Credentials>(query, queryArgs);
                allCredentials.CredentialsList = response.ToList();
            }
            catch (Exception ex)
            {
                allCredentials.CredentialsList = null;
                allCredentials.Error = ex.Message;

                logger.LogError(ex.ToString());
            }

            return allCredentials;
        }

        public async Task<AllCredentials> GetAllCredentials()
        {
            var query = @"select * from Credentials;";
            var allCredentials = new AllCredentials();

            using var connection = context.CreateDefaultConnection();
            try
            {
                var allCredentialsResponse = await connection.QueryAsync<Credentials>(query);
                allCredentials.CredentialsList = allCredentialsResponse.ToList();
            }
            catch (Exception ex)
            {
                allCredentials.CredentialsList = null;
                allCredentials.Error = ex.Message;
            }

            return allCredentials;
        }

        public async Task<Credentials?> GetById(int id)
        {
            var query = $@"select * from {TABLE_NAME} where ""Id"" = @id";
            var queryArgs = new { Id = id };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Credentials>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }

        public async Task<Credentials?> GetByIdAndName(int id, string name)
        {
            var query = $@"select * from {TABLE_NAME} where ""Id""=@id and ""Name""=@name";
            var queryArgs = new { id = id, name = name };
            using var connection = context.CreateDefaultConnection();
            {
                var response = await connection.QueryAsync<Credentials>(query, queryArgs);
                return response.FirstOrDefault();
            }
        }
    }
}