﻿using TgBotApi.Models;

namespace TgBotApi.Repositories.Interfaces;

public interface IMetricRepository
{
    Task<StatDatabase> GetStatDatabaseMetric(string datname);
}