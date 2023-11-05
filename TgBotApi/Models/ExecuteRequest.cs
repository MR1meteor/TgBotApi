namespace TgBotApi.Models
{
    public class ExecuteRequest
    {
        public string Sql { get; set; }
        public long UserId { get; set; }
        public string DatabaseName { get; set; }
    }
}
