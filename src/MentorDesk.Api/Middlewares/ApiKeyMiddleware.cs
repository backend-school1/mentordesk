namespace MentorDesk.Api.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["Api:Key"];
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/health" ||
            context.Request.Path == "/health/ready")
        {
            await _next(context);
            return;
        }

        var providedKey = context.Request.Headers["X-Api-Key"].ToString();

        if (string.IsNullOrWhiteSpace(_apiKey) || providedKey != _apiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
