using System.Net;
using MentorDesk.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MentorDesk.Tests;

public class HealthEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory.WithWebHostBuilder(builder =>
    {
        // Тестам не нужны ни БД, ни Telegram.
        builder.UseSetting("Database:MigrateOnStartup", "false");
        builder.UseSetting("Telegram:BotToken", "");
    });

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
