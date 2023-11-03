using Dapper;
using Npgsql;
using System.Data;
using TgBotApi.Data.Interfaces;

namespace TgBotApi.Data
{
    public class Connection : IConnection
    {
        private readonly string dbConnection;
        public Connection(string dbConnection)
        {
            this.dbConnection = dbConnection;
        }
        public async Task Command(IQueryObject queryObject)
        {
            await Execute(query => query.ExecuteAsync(queryObject.Sql, queryObject.Params)).ConfigureAwait(false);
        }
        public async Task<T> CommandWithResponse<T>(IQueryObject queryObject)
        {
            return await Execute(query => query.QueryFirstAsync<T>(queryObject.Sql, queryObject.Params)).ConfigureAwait(false);
        }
        public async Task<T?> FirstOrDefault<T>(IQueryObject queryObject)
        {
            return await Execute(query => query.QueryFirstOrDefaultAsync<T>(queryObject.Sql, queryObject.Params)).ConfigureAwait(false);
        }
        public async Task<List<T>> ListOrEmpty<T>(IQueryObject queryObject)
        {
            return (await Execute(query => query.QueryAsync<T>(queryObject.Sql, queryObject.Params)).ConfigureAwait(false)).ToList();
        }
        private async Task<T> Execute<T>(Func<IDbConnection, Task<T>> query)
        {
            await using var connection = new NpgsqlConnection(dbConnection);
            T result = await query(connection).ConfigureAwait(false);
            await connection.CloseAsync();
            return result;
        }
    }
}
