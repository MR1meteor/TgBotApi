namespace TgBotApi.Models
{
    public class ExecuteRequest
    {
        public string Sql { get; set; }
        public List<string> Parameters { get; set; }
    }
}
