using MediatR;
using MentorDesk.Domain.Entities;
using MentorDesk.Application.Students;


namespace MentorDesk.Application.Features.Students.Create;

public sealed class CreateStudentsCommandHandler(IStudentRepository repository):IRequestHandler<CreateStudentCommand,Student>
{
    public async Task<Student> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        var userName = TelegramUsernameNormalizer.Normalize(request.TelegramUsername);
        
        var exists = await repository.ExistsByUsernameAsync(userName, ct);
        
        if(exists) return null;

        var student = new Student { TelegramUserName = userName };
        
        await repository.AddAsync(student, ct);
        await repository.SaveChangesAsync(ct);

        return student;

    }
}
