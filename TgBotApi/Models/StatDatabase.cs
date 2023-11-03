namespace TgBotApi.Models;

public class StatDatabase
{
    public string transactionCount { get; set; }
    public string getCount { get; set; }
    public string insertCount { get; set; }
    public string updateCount { get; set; }
    public string deleteCount { get; set; }
    public string returnCount { get; set; }
    public string conflictCount { get; set; }
    public string deadlockSum { get; set; }
    public string sessionTime { get; set; }
    public string sesssionCount { get; set; }
    public string sessionKilledCount { get; set; }
    public string sessionAbandonedCount { get; set; }
}