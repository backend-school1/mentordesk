using System.Text.Json.Serialization;
using MentorDesk.Domain.Enums;

public class StudentsDTO
{
    public sealed record CreateStudentRequest(string telegramUserName);
    public sealed record ChangeStudentStatusRequest(
        [property: JsonConverter(typeof(JsonStringEnumConverter<StudentStatus>))] StudentStatus status);

    public sealed record StudentResponse(
        Guid id,
        string? telegramUserName,
        long? telegramUserId,
        string status,
        DateTime createdAt);
}
