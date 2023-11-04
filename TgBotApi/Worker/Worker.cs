using KafkaClient.Interfaces;
using KafkaClient.Models;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Worker;
public sealed class Worker : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IKafkaProducesService kafkaProducesService;

    public Worker(IServiceProvider serviceProvider, IKafkaProducesService logger) {
        this.serviceProvider = serviceProvider;
        this.kafkaProducesService = logger;
    }
    
    private async Task<List<StateChange>> LogAllErrorStatuses(CancellationToken stoppingToken)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IActivityRepository scopedProcessingService =
                scope.ServiceProvider.GetRequiredService<IActivityRepository>();

            return await scopedProcessingService.GetAllErrorStatus();
        }
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // var resStatuses = await LogAllErrorStatuses(stoppingToken);
            // if (resStatuses.Count > 0)
            // {
            //     var message = new Message();
            //     message.Object = resStatuses;
            //     message.MessageType = "AllLocks";
            //     await kafkaProducesService.WriteTraceLogAsync(message);
            // }
            await Task.Delay(15_000, stoppingToken);
        }
    }
}