using MentorDesk.Api.Bot;
using MentorDesk.Application;
using MentorDesk.Infrastructure;
using MentorDesk.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Telegram.Bot;
using Telegram.Bot.Types;

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
var apiKey = builder.Configuration["Api:Key"];
builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddHostedService<BotHostedService>();

var app = builder.Build();
app.UseRouting();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health" ||
        context.Request.Path == "/health/ready")
    {
        await next(context);
        return;
    }

    var providedKey = context.Request.Headers["X-Api-Key"].ToString();

    if (string.IsNullOrWhiteSpace(apiKey) || providedKey != apiKey)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }
    await next(context);
});

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

app.MapControllers();

app.Run();

public partial class Program;
