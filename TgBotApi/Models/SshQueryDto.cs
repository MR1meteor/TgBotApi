namespace TgBotApi.Models
{
    public class SshQueryDto
    {
        public string Query { get; set; }
        public string QueryName { get; set; }
        public long UserId { get; set; }
        public string DatabaseName { get; set; }
    }
}
