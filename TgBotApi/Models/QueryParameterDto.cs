namespace TgBotApi.Models
{
    public class QueryParameterDto
    {
        public long UserId { get; set; }
        public string DatabaseName { get; set; }
        public string QueryName { get; set; }
        public string Parameter { get; set; } = string.Empty;
    }
}
