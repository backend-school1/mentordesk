using System.Net;
using MentorDesk.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MentorDesk.Tests;

public class ApiKeyTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _developmentFactory =
        factory.WithWebHostBuilder(builder =>
        {
            
            builder.UseSetting(
                "ConnectionStrings:Postgres",
                "Host=127.0.0.1;Database=mentordesk_test;Username=test;Password=test");
            
            builder.UseEnvironment("Development");
            builder.UseSetting("Database:MigrateOnStartup", "false");
            builder.UseSetting("Telegram:BotToken", "");
            builder.UseSetting("Api:DisableKeyValidation", "true");
        });

    private readonly WebApplicationFactory<Program> _securedFactory =
        factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Staging");

            builder.UseSetting(
                "ConnectionStrings:Postgres",
                "Host=127.0.0.1;Database=mentordesk_test;Username=test;Password=test");

            builder.UseSetting("Database:MigrateOnStartup", "false");
            builder.UseSetting("Telegram:BotToken", "");
            builder.UseSetting("Api:Key", "test-only-api-key");
            builder.UseSetting("Api:DisableKeyValidation", "false");
        });

    [Fact]
    public async Task Import_WithoutApiKey_ReturnsUnauthorized()
    {
        var client = _securedFactory.CreateClient();

        var response = await client.PostAsync(
       $"/api/Decks/{Guid.Empty}/import",
       null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Import_WithWrongApiKey_ReturnsUnauthorized()
    {
        var client = _securedFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "wrong-key");
        var response = await client.PostAsync(
       $"/api/Decks/{Guid.Empty}/import",
       null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Swagger_IsAccessibleWithoutApiKey_InDevelopment()
    {
        var client = _developmentFactory.CreateClient();

        var uiResponse =
            await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, uiResponse.StatusCode);

        var jsonResponse =
            await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, jsonResponse.StatusCode);
    }
    
    [Fact]
    public async Task Readiness_WithoutApiKey_ReturnsHealthStatus()
    {
        var client = _securedFactory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.ServiceUnavailable);
    }
    
    [Fact]
    public async Task Development_WithoutApiKey_PassesMiddleware()
    {
        var client = _developmentFactory.CreateClient();

        var response =
            await client.GetAsync("/api/nonexistent");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Staging_WithCorrectApiKey_PassesMiddleware()
    {
        var client = _securedFactory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Api-Key",
            "test-only-api-key");

        var response =
            await client.GetAsync("/api/nonexistent");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
