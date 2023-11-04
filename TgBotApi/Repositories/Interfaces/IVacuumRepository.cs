using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces;

public interface IVacuumRepository
{
    Task<VacuumFullRefresh> VacuumFull(Credentials credentials);
}