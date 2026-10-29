using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MentorDesk.Tests;

public class ApiKeyTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory.WithWebHostBuilder(builder =>
    {
        builder.UseSetting("Database:MigrateOnStartup", "false");
        builder.UseSetting("Telegram:BotToken", "");
        builder.UseSetting("Api:Key", "test-only-api-key");
    });

    [Fact]
    public async Task Import_WithoutApiKey_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync(
       $"/api/Decks/{Guid.Empty}/import",
       null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task Import_WithWrongApiKey_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "wrong-key");
        var response = await client.PostAsync(
       $"/api/Decks/{Guid.Empty}/import",
       null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_RequiresCorrectApiKey()
    {
        var client = _factory.CreateClient();

        var withoutKey = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.Unauthorized, withoutKey.StatusCode);

        client.DefaultRequestHeaders.Add("X-Api-Key", "test-only-api-key");

        var withKey = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, withKey.StatusCode);
    }
    [Fact]
    public async Task Readiness_WithoutApiKey_ReturnsHealthStatus()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.ServiceUnavailable);
    }
}
