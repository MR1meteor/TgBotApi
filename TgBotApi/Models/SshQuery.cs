namespace TgBotApi.Models;

public class SshQuery
{
    public int Id { get; set; }
    public string Query { get; set; }
    public string QueryName { get; set; }
    public int CredentialId { get; set; }
}