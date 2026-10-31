using System.Text.Json.Serialization;
using MentorDesk.Domain.Enums;


public sealed record CreateStudentRequest(string TelegramUserName);
public sealed record ChangeStudentStatusRequest(StudentStatus Status);
public sealed record StudentResponse(
    Guid Id,
    string? TelegramUserName,
    long? TelegramId,
    string Status,
    DateTime CreatedAt);
