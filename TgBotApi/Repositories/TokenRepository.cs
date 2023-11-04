using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private const string TABLE_NAME = "public.tokens";
        private readonly DapperContext context;

        public TokenRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<Token?> GetByUser(long userId)
        {
            var query = $@"select * from {TABLE_NAME} where ""UserId"" = @userId";
            var queryArgs = new { UserId = userId };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Token>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }

        public async Task<Token?> GetByToken(string token)
        {
            var query = $@"select * from {TABLE_NAME} where ""TokenValue"" = @tokenValue";
            var queryArgs = new { TokenValue = token };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Token>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }

        public async Task<Token?> Add(Token token)
        {
            var query = $@"insert into {TABLE_NAME} (""UserId"", ""TokenValue"", ""LastLogin"")
                            values (@userId, @tokenValue, @lastLogin)";
            var queryArgs = new { UserId = token.UserId, TokenValue = token.TokenValue, LastLogin = token.LastLogin };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Token>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }

        public async Task<Token?> Update(Token token)
        {
            var query = $@"update {TABLE_NAME}
                            set ""Token"" = @token, ""LastLogin"" = @lastLogin
                            where ""UserId"" = @userId
                            returning *";

            var queryArgs = new { Token = token.TokenValue, LastLogin = token.LastLogin, UserId = token.UserId };

            using (var connection = context.CreateDefaultConnection())
            {
                var response = await connection.QueryAsync<Token>(query, queryArgs);

                return response?.FirstOrDefault();
            }
        }
    }
}
