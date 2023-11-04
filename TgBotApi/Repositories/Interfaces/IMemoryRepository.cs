using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces;

public interface IMemoryRepository
{
    Task<bool> UpdateMemoryCorrelations(MemoryCorrelations memoryCorrelations, int userId, string name);
    Task<MemoryCorrelations> GetMemoryCorrelations(int userId, string name);
    // Task<string?> CreateDatabaseDump(int userId, string name);
}