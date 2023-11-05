namespace TgBotApi.Models
{
    public class ExecuteSshDto
    {
        public string Query { get; set; }
        public long UserId { get; set; }
        public string DatabaseName { get; set; }
    }
}
