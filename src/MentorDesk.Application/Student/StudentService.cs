using System;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;

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

    public async Task<List<Student>> GetAllAsync(CancellationToken ct=default)
    {
        return await repository.GetAllAsync(ct);
    }

    public async Task<Student?> ChangeStatusAsync(Guid id, StudentStatus newStatus, CancellationToken ct = default)
    {
        var student = await repository.GetByIdAsync(id, ct);

        if (student is null)
            return null;

        if (!Enum.IsDefined(newStatus))
            throw new ArgumentOutOfRangeException(nameof(newStatus));

        if (student.Status != newStatus)
        {
            student.Status = newStatus;
            await repository.SaveChangesAsync(ct);
        }

        return student;
    }
}
