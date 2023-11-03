namespace TgBotApi.Models
{
    public class Credentials
    {
        public string Name {  get; set; }
        public long UserId { get; set; }
        public string Host {  get; set; }
        public string Port {  get; set; }
        public string Database {  get; set; }
        public string Username {  get; set; }
        public string Password { get; set; }
    }

    public class AllCredentials
    {
        private List<Credentials> CredentialsList { get; set; }
    }
}
