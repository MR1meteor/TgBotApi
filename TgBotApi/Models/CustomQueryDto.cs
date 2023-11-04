namespace TgBotApi.Models
{
    public class CustomQueryDto
    {
        public long UserId { get; set; }
        public string DatabaseName {  get; set; }
        public string Sql { get; set; }
        public string QueryName { get; set; }
    }
}
