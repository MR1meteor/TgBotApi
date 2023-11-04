namespace TgBotApi.Models
{
    public class LinkModel
    {
        public int Id { get; set; }
        public int CredentialId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
