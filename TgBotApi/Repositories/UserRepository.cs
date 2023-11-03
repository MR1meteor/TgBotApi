using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string TABLE_NAME = @"""public"".""Users""";
        private readonly IConfiguration configuration;

        public UserRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        //public Task<bool> Add(User user)
        //{
        //    var query = $@"";
        //}
    }
}
