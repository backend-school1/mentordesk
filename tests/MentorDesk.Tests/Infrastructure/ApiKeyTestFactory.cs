using MentorDesk.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MentorDesk.Tests.Infrastructure;

public sealed class ApiKeyTestFactory(
    string environment,
    bool disableKeyValidation)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment(environment);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Postgres"] =
                        "Host=127.0.0.1;Port=1;Database=mentordesk_test;Username=test;Password=test;Timeout=1",

                    ["Database:MigrateOnStartup"] = "false",
                    ["Telegram:BotToken"] = "",
                    ["Api:Key"] = "test-only-api-key",
                    ["Api:DisableKeyValidation"] =
                        disableKeyValidation.ToString()
                });
        });

        return base.CreateHost(builder);
    }
}
