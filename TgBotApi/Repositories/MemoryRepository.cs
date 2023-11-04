using System.Diagnostics;
using System.Drawing.Printing;
using Dapper;
using KafkaClient.Interfaces;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services.Interfaces;

namespace TgBotApi.Repositories;

public class MemoryRepository: IMemoryRepository
{
    private readonly DapperContext context;
    private readonly ICredentialsRepository credentialsRepository;
    private readonly ISshService sshRepository;
    private readonly IKafkaProducesService kafkaProducesService;
    
    public MemoryRepository(DapperContext context, ICredentialsRepository credentialsRepository, ISshService sshRepository, IKafkaProducesService kafkaProducesService)
    {
        this.context = context;
        this.credentialsRepository = credentialsRepository;
        this.sshRepository = sshRepository;
        this.kafkaProducesService = kafkaProducesService;
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

    // public async Task<string?> CreateDatabaseDump(int userId, string name)
    // {
    //         var credentials = await credentialsRepository.GetByIdAndName(userId, name);
    //         var pgDumpCommand = $"pg_dump -U test > test";
    //         var catCommand = $"cat test";
    //
    //         using (var connection = sshRepository.CreateConnection(new SshConnect()
    //                {
    //                    Ip = credentials.Host,
    //                    Password = credentials.Password,
    //                    Port = credentials.Port,
    //                    Username = credentials.Username,
    //                }))
    //         {
    //             await sshRepository.ExecuteCommandWithOutOutput(pgDumpCommand, connection);
    //             var response = await sshRepository.ExecuteCommand(catCommand, connection);
    //             return response;
    //         }
    // }
}