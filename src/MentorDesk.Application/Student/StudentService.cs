using System;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Students;
/// <summary>
/// StudentService class provides operations related to student management in the MentorDesk system.
/// </summary>
public class StudentService(IStudentRepository repository)
{
    public async Task<Student?> CreateAsync(
        string rawUsername,
        CancellationToken ct = default)
    {
        var username =
            TelegramUsernameNormalizer.Normalize(rawUsername);

        var exists = await repository
            .ExistsByUsernameAsync(username, ct);

        if (exists)
            return null;

        var student = new Student()
        {
            TelegramUserName = username
        };

        await repository.AddAsync(student, ct);
        await repository.SaveChangesAsync(ct);

        return student;
    }
}
