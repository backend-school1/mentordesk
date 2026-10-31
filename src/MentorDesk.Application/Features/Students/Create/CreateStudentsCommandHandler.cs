using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Domain.Entities;


namespace MentorDesk.Application.Features.Students.Create;

public sealed class CreateStudentsCommandHandler(IStudentRepository repository)
    : IRequestHandler<CreateStudentCommand, Result<Student>>
{
    public async Task<Result<Student>> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        var userName = TelegramUsernameNormalizer.Normalize(request.TelegramUserName);

        var exists = await repository.ExistsByUsernameAsync(userName, ct);

        if (exists) return Result<Student>.Failure("Student already exists");

        var student = new Student { TelegramUserName = userName };

        await repository.AddAsync(student, ct);
        await repository.SaveChangesAsync(ct);

        return Result<Student>.Success(student);

    }
}
