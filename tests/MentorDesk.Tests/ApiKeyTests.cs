using System.Net;
using MentorDesk.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MentorDesk.Tests;

public class ApiKeyTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory =
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
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
    public async Task Swagger_IsAccessibleWithoutApiKey_InDevelopment()
    {
        var client = _factory.CreateClient();

        var uiResponse = await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, uiResponse.StatusCode);

        var jsonResponse = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, jsonResponse.StatusCode);
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
