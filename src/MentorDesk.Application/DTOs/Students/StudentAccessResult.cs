namespace MentorDesk.Application.Features.Students.Access;

public enum StudentAccessStatus
{
    Allowed,
    RequiresBinding,
    Denied
}

public sealed record StudentAccessResult(StudentAccessStatus Status, Guid? StudentId);

