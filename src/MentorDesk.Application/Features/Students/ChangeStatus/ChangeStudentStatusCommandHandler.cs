using MediatR;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.ChangeStatus;

public sealed class ChangeStudentStatusCommandHandler(
    IStudentRepository repository)
    : IRequestHandler<ChangeStudentStatusCommand, Student?>
{
    public async Task<Student?> Handle(
        ChangeStudentStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Status))
            throw new ArgumentOutOfRangeException(
                nameof(request.Status));

        var student = await repository.GetByIdAsync(
            request.StudentId,
            cancellationToken);

        if (student is null)
            return null;

        if (student.Status != request.Status)
        {
            student.Status = request.Status;

            await repository.SaveChangesAsync(
                cancellationToken);
        }

        return student;
    }
}
