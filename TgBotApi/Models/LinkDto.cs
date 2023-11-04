namespace TgBotApi.Models
{
    public class LinkDto
    {
        public long UserId { get; set; }
        public string LinkName { get; set; } // Link name given by user
        public string DatabaseName { get; set; } // Database name given by user
        public string Url { get; set; } = string.Empty;
    }
}
