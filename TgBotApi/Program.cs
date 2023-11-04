using DbUp;
using System.Reflection;
using Common;
using Common.Interfaces;
using KafkaClient.Interfaces;
using KafkaClient.Services;
using TgBotApi.Data;
using TgBotApi.Repositories;
using TgBotApi.Repositories.Interfaces;
using TgBotApi.Services;
using TgBotApi.Services.Interfaces;
using TgBotApi.Worker;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetSection("DbConnections").GetSection("OnwDataBasePostgress").Value;

var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

upgrader.PerformUpgrade();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<IConfigurationSettings, ConfigurationSettings>();
builder.Services.AddSingleton<IKafkaProducesService, KafkaProducesService>();


builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<ICredentialsRepository, CredentialsRepository>();
builder.Services.AddScoped<IVisualService, VisualService>();
builder.Services.AddScoped<IMetricRepository, MetricRepository>();
builder.Services.AddScoped<IVacuumRepository, VacuumRepository>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddHostedService<Worker>();


var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
