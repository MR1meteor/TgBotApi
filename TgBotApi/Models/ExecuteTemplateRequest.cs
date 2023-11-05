namespace TgBotApi.Models
{
    public class ExecuteTemplateRequest
    {
        public string CredentialsName { get; set; }
        public long UserId { get; set; }
        public string QueryName { get; set; }
        public List<string> Parameters { get; set; }
    }
}
