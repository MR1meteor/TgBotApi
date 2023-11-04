using Npgsql;
using System.Data;
using Common.Interfaces;
using TgBotApi.Models;

namespace TgBotApi.Data
{
    public class DapperContext
    {
        private readonly string connectionString;

        public DapperContext(IConfigurationSettings configuration)
        {

            this.connectionString = configuration.DbConnectionsOwn;
        }

        public IDbConnection CreateUserConnection(Credentials credentials)
        {
            var connectionString = $@"Host={credentials.Host};Port={credentials.Port};Database={credentials.Database};User Id={credentials.Username};Password={credentials.Password}";

            return new NpgsqlConnection(connectionString);
        }

        public IDbConnection CreateDefaultConnection() 
            => new NpgsqlConnection(connectionString);
    }
}
    