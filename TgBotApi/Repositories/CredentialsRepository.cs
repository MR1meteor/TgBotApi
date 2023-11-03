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

        public CredentialsRepository(DapperContext context)
        {
            this.context = context;
        }
        
        public async Task<bool> Add(Credentials creds)
        {
            var query = $@"insert into {TABLE_NAME} ({nameof(Credentials.Name)}, {nameof(Credentials.UserId)}, {nameof(Credentials.Host)},
                            {nameof(Credentials.Port)}, {nameof(Credentials.Database)}, {nameof(Credentials.Username)}, {nameof(Credentials.Password)})
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
            var query = $@"select * from {TABLE_NAME} where {nameof(Credentials.Name)} = @name and {nameof(Credentials.UserId)} = @userId";

            var queryArgs = new { Name = name, UserId = userId };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Credentials>(query, queryArgs);

                return response.FirstOrDefault();
            }
        }

        public async Task<List<Credentials>> GetByUser(long userId)
        {
            var query = $@"select * from {TABLE_NAME} where {nameof(Credentials.UserId)} = @userId";

            var queryArgs = new { UserId = userId };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Credentials>(query, queryArgs);

                return response.ToList();
            }
        }
    }
}
