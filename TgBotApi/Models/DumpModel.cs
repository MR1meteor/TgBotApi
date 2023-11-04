namespace TgBotApi.Models;

public class DumpModel
{
    public int UserId { get; set; }
    public int CredentialId { get; set; }
    public DateTime EventDate;
    public string SQL { get; set; } 
}