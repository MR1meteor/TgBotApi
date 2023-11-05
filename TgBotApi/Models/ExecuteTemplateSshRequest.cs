namespace TgBotApi.Models
{
    public class ExecuteTemplateSshRequest
    {
        public string DatabaseName { get; set; }
        public long UserId { get; set; }
        public string QueryName { get; set; }
    }
}
