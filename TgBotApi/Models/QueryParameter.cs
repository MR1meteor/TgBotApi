namespace TgBotApi.Models
{
    public class QueryParameter
    {
        public int Id { get; set; }
        public int QueryId { get; set; }
        public string Parameter { get; set; } = string.Empty;
    }
}
