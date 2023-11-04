using Common;
using Common.Interfaces;
using Confluent.Kafka;
using KafkaClient.Interfaces;
using Microsoft.Extensions.Logging;


namespace KafkaClient.Services;

public class KafkaProducesService : IKafkaProducesService
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducesService> _logger;
    private readonly string _topic;

    public KafkaProducesService(IConfigurationSettings settings, ILogger<KafkaProducesService> logger)
    {
        _logger = logger;
        _topic = settings.KafkaTopic;
        _producer = new ProducerBuilder<Null, string>(new ProducerConfig
        {
            LingerMs = 2000,
            BatchSize = 1000,
            QueueBufferingMaxMessages = 500,
            BootstrapServers = settings.KafkaConnection
        }).SetLogHandler((_, logMessage) =>
        {
            if (logMessage.Level != SyslogLevel.Info && logMessage.Level != SyslogLevel.Debug)
            {
                _logger.LogError($"Kafka producer error. {logMessage.Message}");
            }
        }).Build();
    }

    public async Task WriteTraceLogAsync(object value)
    {
        try
        {
            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = value.ToJson() });
        }
        catch (ProduceException<Null, string> exc)
        {
            _logger.LogError($"Send message to kafka failed: {exc.Error}. Message: {value.ToJson()}");
            _ = Task.CompletedTask;
        }
    }
}