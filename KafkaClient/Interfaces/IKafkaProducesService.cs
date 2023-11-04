namespace KafkaClient.Interfaces;

public interface IKafkaProducesService
{
    Task WriteTraceLogAsync(object value);
}