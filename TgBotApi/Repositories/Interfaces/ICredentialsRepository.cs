﻿using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces
{
    public interface ICredentialsRepository
    {
        Task<bool> Add(Credentials creds);
        Task<Credentials?> Get(string dbname, long userId);
        Task<Credentials?> GetById(int id);
        Task<AllCredentials> GetByUser(long userId);
        Task<AllCredentials> GetAllCredentials();
        Task<Credentials?> GetByIdAndName(int id, string name);
    }
}
