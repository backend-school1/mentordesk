using MediatR;
using MediatR.Pipeline;
using MentorDesk.Application;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Students.Access;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;


namespace MentorDesk.Application.Features.Students.Access;

public sealed class CheckStudentAccessQueryHandler(IStudentRepository repository) : IRequestHandler<CheckStudentAccessQuery, StudentAccessResult>
{
    public async Task<StudentAccessResult> Handle(CheckStudentAccessQuery request, CancellationToken ct)
    {
        var student = await repository.GetByTelegramIdAsync(request.TelegramId, ct);

        if (student is not null)
        {
            return student.Status == StudentStatus.Active
                ? new StudentAccessResult(StudentAccessStatus.Allowed, student.Id)
                : new StudentAccessResult(StudentAccessStatus.Denied, null);
        }

        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return new StudentAccessResult(StudentAccessStatus.Denied, null);
        }

        var username = TelegramUsernameNormalizer.Normalize(request.UserName);

        student = await repository.GetByUsernameAsync(username, ct);

        if (student is null || student.Status != StudentStatus.Active)
        {
            return new StudentAccessResult(StudentAccessStatus.Denied, null);
        }

        if (student.TelegramId is not null)
        {
            return new StudentAccessResult(StudentAccessStatus.Denied, null);
        }

        return new StudentAccessResult(StudentAccessStatus.RequiresBinding, student.Id);

    }
}


