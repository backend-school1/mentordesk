using MentorDesk.Api.Bot;
using MentorDesk.Api.Middlewares;
using MentorDesk.Application;
using MentorDesk.Infrastructure;
using MentorDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:Postgres");
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database", tags: ["ready"]);

var botToken = builder.Configuration["Telegram:BotToken"];
if (!string.IsNullOrWhiteSpace(botToken))
{
    builder.Services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));
}
builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddHostedService<BotHostedService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseRouting();

app.UseMiddleware<ApiKeyMiddleware>();

if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// /health — процесс жив; /health/ready — ещё и БД доступна.
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.UseExceptionHandler();
app.MapControllers();

app.Run();

namespace MentorDesk.Api
{
    public partial class Program;
}
