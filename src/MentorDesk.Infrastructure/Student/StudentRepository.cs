using System;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;
using MentorDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MentorDesk.Infrastructure;

/// <summary>
/// 
/// </summary>
public sealed class StudentRepository(AppDbContext db) : IStudentRepository
{
    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await db.Students.AnyAsync(s => s.TelegramUserName == username, ct);
    }

    public async Task AddAsync(Student student, CancellationToken ct = default)
    {
        await db.Students.AddAsync(student, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await db.SaveChangesAsync(ct);
    }

    public async Task<List<Student>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Students.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Students.FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
