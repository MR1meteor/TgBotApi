﻿namespace TgBotApi.Models
{
    public class AddSshConnectionDbo
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long UserId { get; set; }
        public string DatabaseName { get; set; }
    }
}
