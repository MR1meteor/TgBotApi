using Dapper;
using TgBotApi.Data;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Repositories;

public class VacuumRepository: IVacuumRepository
{
    private const string TABLE_NAME = @"public.credentials";
    private readonly DapperContext context;
    private readonly ILogger<MetricRepository> logger;
    private readonly IMetricRepository metricRepository;

    public VacuumRepository(DapperContext context, ILogger<MetricRepository> logger, IMetricRepository metricRepository)
    {
        this.context = context;
        this.logger = logger;
        this.metricRepository = metricRepository;
    }

    public async Task<VacuumFullRefresh> VacuumFull(Credentials credentials)
    {
        var vacuumFullRefresh = new VacuumFullRefresh();
        vacuumFullRefresh.BeforeMemory = await metricRepository.GetDatabaseMemory(credentials);

        var query = @$"vacuum full";
        
        using var connection = context.CreateUserConnection(credentials);
        {
            await connection.ExecuteAsync(query);
            vacuumFullRefresh.AfterMemory = await metricRepository.GetDatabaseMemory(credentials);
        }

        return vacuumFullRefresh;
    }
}