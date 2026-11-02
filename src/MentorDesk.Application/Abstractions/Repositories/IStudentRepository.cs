using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Abstractions.Repositories;

/// <summary>
/// Student repository interface for managing student entities in the MentorDesk system.
/// </summary>
public interface IStudentRepository
{
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);

    Task AddAsync(Student student, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);

    Task<List<Student>> GetAllAsync(CancellationToken ct = default);

    Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default);

}
