namespace TgBotApi.Models
{
    public class Token
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string TokenValue { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
