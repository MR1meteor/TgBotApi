using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces;

public interface IMetricRepository
{
    Task<StatDatabase> GetStatDatabaseMetric(int userId, string name);
    Task<string?> GetDatabaseMemory(Credentials? credentials);
}