namespace TgBotApi.Models;

public class StateChange
{
    public int Pid { get; set; }
    public string State { get; set; }
    public DateTimeOffset StateLastChangeDate { get; set; }
    public string WaitEventType { get; set; }
    public string DataBase { get; set; }
    public long userId { get; set; }
}