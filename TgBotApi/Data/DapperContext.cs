using Npgsql;
using System.Data;
using TgBotApi.Models;

namespace TgBotApi.Data
{
    public class DapperContext
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public DapperContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateUserConnection(Credentials credentials)
        {
            var connectionString = $@"Host={credentials.Database};Port={credentials.Port};Database={credentials.Database};
                                        User Id={credentials.Username};Password={credentials.Password}";

            return new NpgsqlConnection(connectionString);
        }

        public IDbConnection CreateDefaultConnection() 
            => new NpgsqlConnection(connectionString);
    }
}
    