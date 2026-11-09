namespace MentorDesk.Api.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _apiKey;
    private readonly bool _disableKeyValidation;

    public ApiKeyMiddleware(RequestDelegate next,
        IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["Api:Key"];

        _disableKeyValidation = configuration.GetValue<bool>("Api:DisableKeyValidation");
    }

    public async Task InvokeAsync(
        HttpContext context,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment() && _disableKeyValidation)
        {
            await _next(context);
            return;
        }

        if (context.Request.Path == "/health" ||
            context.Request.Path == "/health/ready")
        {
            await _next(context);
            return;
        }

        var providedKey = context.Request.Headers["X-Api-Key"].ToString();

        if (string.IsNullOrWhiteSpace(_apiKey) ||
            providedKey != _apiKey)
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
