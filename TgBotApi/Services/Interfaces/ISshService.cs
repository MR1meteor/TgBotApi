﻿using Microsoft.Data.SqlClient;
using Renci.SshNet;
using TgBotApi.Models;

namespace TgBotApi.Services.Interfaces;

public interface ISshService
{
    Task<string> CheckDiskSpace(int userId);
    Task<string> Lsof(int userId);
    Task<string> Tcpdump(int userId);
    void AddQuery(SshQuery query);
    Task<List<SshQuery>> GetQuery(int credentialsId);
    void DeleteQuery(int credentialsId, string queryName);
    Task<SshQuery> UpdateQuery(SshQuery query);
    Task<string> CreateDump(int userId, string name);
    Task<bool> LoadDump(int dumpId, int userId);
    Task<List<CredentialAndDatabase>> GetAllConnections(int userId);
    Task<bool> CreateSshConnectionOnCredential(SshConnect connect);
    Task<SshConnect?> GetSshConnection(int credentialsId);
    Task<List<DumpModel>> GetDumpsByUserId(int userId, string name);
    Task<ExecuteResponse> Execute(SshConnect connect, string query);
    Task<SshQuery?> GetQueryByCreds(int credentialsId, string queryName);
}