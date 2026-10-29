using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;

namespace MentorDesk.Api.Students;

public class StudentsDTO
{
    public sealed record CreateStudentRequest(string telegramUserName);
    public sealed record ChangeStudentStatusRequest(string status);

    public sealed record StudentResponse(
        Guid id,
        string? telegramUserName,
        long? telegramUserId,
        string status,
        DateTime createdAt);
}
