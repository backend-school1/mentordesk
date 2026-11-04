using System.Text.Json.Serialization;


public sealed record StudentResponse(
    Guid Id,
    string? TelegramUserName,
    long? TelegramId,
    string Status,
    DateTime CreatedAt);
