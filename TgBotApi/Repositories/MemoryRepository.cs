using System.Diagnostics;
using System.Drawing.Printing;
using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories;

public class MemoryRepository: IMemoryRepository
{
    private readonly DapperContext context;
    private readonly ICredentialsRepository credentialsRepository;
    
    public MemoryRepository(DapperContext context, ICredentialsRepository credentialsRepository)
    {
        this.context = context;
        this.credentialsRepository = credentialsRepository;
    }
    
    public async Task<bool> UpdateMemoryCorrelations(MemoryCorrelations memoryCorrelations, int userId, string name)
    {
        Credentials credentials = await credentialsRepository.GetByIdAndName(userId, name);

        try
        {
            using (var connection = context.CreateUserConnection(credentials))
            {

                var query = $@"begin;
                            set maintenance_work_mem to '{memoryCorrelations.MaintenanceWorkMemory}';
                            set effective_cache_size to '{memoryCorrelations.EffectiveCacheSize}';
                            set Work_Mem to '{memoryCorrelations.WorkMemory}';
                           commit;";
                await connection.ExecuteAsync(query);
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<MemoryCorrelations> GetMemoryCorrelations(int userId, string name)
    {
        var credentials = await credentialsRepository.GetByIdAndName(userId, name);
        var memoryCorrelations = new MemoryCorrelations();
        using (var connection = context.CreateUserConnection(credentials))
        {
            memoryCorrelations.WorkMemory = await connection.QueryFirstOrDefaultAsync<string>(@"show Work_mem");
            memoryCorrelations.MaintenanceWorkMemory = await connection.QueryFirstOrDefaultAsync<string>(@"show maintenance_work_mem");
            memoryCorrelations.EffectiveCacheSize = await connection.QueryFirstOrDefaultAsync<string>(@"show effective_cache_size");
            memoryCorrelations.MemoryLimit  = await connection.QueryFirstOrDefaultAsync<string>(@"show Shared_buffers ");
            return memoryCorrelations;
        }
    }

    public async Task<string?> CreateDatabaseDump(int userId, string name)
    {
        try
        {

            var credentials = await credentialsRepository.GetByIdAndName(userId, name);
            var commandLine = string.Format($"pg_dump -h {credentials.Host} -U {credentials.Username} -W {credentials.Password} {credentials.Database} > mysql.sql");

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = string.Format( "/c \"{0}\"", commandLine)
                };
                process.Start();
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
}