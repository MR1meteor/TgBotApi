using Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Common;

public class ConfigurationSettings : IConfigurationSettings
{
    private readonly IConfiguration configuration;

    public ConfigurationSettings(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string DbConnectionsOwn => configuration.GetSection("DbConnections").GetSection("OnwDataBasePostgress").Value;

    public string KafkaConnection => configuration.GetSection("Kafka").GetSection("Connection").Value;
    

    public string KafkaTopic => configuration.GetSection("Kafka").GetSection("Topic").Value;
}