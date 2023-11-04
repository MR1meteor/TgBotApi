namespace TgBotApi.Models;

public class MemoryCorrelations
{
    public string EffectiveCacheSize { get; set; } = "4GB";
    public string MaintenanceWorkMemory { get; set; } = "256MB";
    public string WorkMemory { get; set; } = "4MB";
    public string MemoryLimit { get; set; }
}