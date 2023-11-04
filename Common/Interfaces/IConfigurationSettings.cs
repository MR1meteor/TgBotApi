namespace Common.Interfaces;

public interface IConfigurationSettings
{
    public string DbConnectionsOwn { get; }

    public string KafkaConnection { get; }
    public string KafkaTopic { get; }
}