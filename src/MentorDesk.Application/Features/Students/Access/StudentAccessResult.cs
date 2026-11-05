namespace MentorDesk.Application.Features.Students.Access;

public sealed record StudentAccessResult(StudentAccessStatus Status, Guid? StudentId)
{
    
}
