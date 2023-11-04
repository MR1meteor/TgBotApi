namespace TgBotApi.Models
{
    public class CustomQuery
    {
        public int Id { get; set; }
        public int CredentialsId { get; set; }
        public string Sql { get; set; }
    }
}
