using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace TgBotApi.Models;

public class VacuumFullRefresh
{
    public string? BeforeMemory { get; set; }
    public string? AfterMemory { get; set; }
}