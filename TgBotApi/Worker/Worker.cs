using KafkaClient.Interfaces;
using KafkaClient.Models;
using Microsoft.OpenApi.Writers;
using TgBotApi.Models;
using TgBotApi.Repositories.Interfaces;

namespace TgBotApi.Worker;

public sealed class Worker : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IKafkaProducesService kafkaProducesService;

    public Worker(IServiceProvider serviceProvider, IKafkaProducesService logger)
    {
        this.serviceProvider = serviceProvider;
        this.kafkaProducesService = logger;
    }

    private async Task<AllCredentials> GetAllCredentials()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            var scopedRepository = scope.ServiceProvider.GetRequiredService<ICredentialsRepository>();
            return await scopedRepository.GetAllCredentials();
        }
    }

    private async Task<List<StateChange>> GetErrorStatus(Credentials credentials)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            var scopedRepository = scope.ServiceProvider.GetRequiredService<IActivityRepository>();
            return await scopedRepository.GetErrorStatus(credentials);
        }
    }

    private async Task<List<StateChange>> LogAllErrorStatuses(Credentials credentials)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IActivityRepository scopedProcessingService =
                scope.ServiceProvider.GetRequiredService<IActivityRepository>();

            return await scopedProcessingService.GetErrorStatus(credentials);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var allCredentials = await GetAllCredentials();
            foreach (var credential in allCredentials.CredentialsList)
            {
                try
                {
                    var response = await GetErrorStatus(credential);
                    if (response.Count > 0)
                    {
                        var message = new Message();
                        message.Object = response;
                        message.MessageType = "LockStatus";
                        await kafkaProducesService.WriteTraceLogAsync(message);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            await Task.Delay(15_000, stoppingToken);
        }
    }
}