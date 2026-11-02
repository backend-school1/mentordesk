using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.ChangeStatus;

public sealed class ChangeStudentStatusCommandHandler(IStudentRepository repository)
    : IRequestHandler<ChangeStudentStatusCommand, Result<Student>>
{
    public async Task<Result<Student>> Handle(ChangeStudentStatusCommand request,
        CancellationToken cancellationToken)
    {

        var student = await repository.GetByIdAsync(
            request.StudentId,
            cancellationToken);

        if (student is null)
        {
            return Result<Student>.Failure(
                "Student not found");
        }

        if (student.Status != request.Status)
        {
            student.Status = request.Status;

            await repository.SaveChangesAsync(
                cancellationToken);
        }

        return Result<Student>.Success(student);
    }
}
