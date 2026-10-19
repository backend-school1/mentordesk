
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;
using Moq;

namespace MentorDesk.Tests;

public sealed class StudentServiceTests
{
    [Fact]
    public async Task ChangeStatus_ActiveToArchived_SavesChanges()
    {
        // Arrange
        var student = new Student
        {
            Status = StudentStatus.Active
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByIdAsync(
                student.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var service = new StudentService(repository.Object);

        // Act
        var result = await service.ChangeStatusAsync(
            student.Id,
            StudentStatus.Archived);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StudentStatus.Archived, result.Status);

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangeStatus_StudentNotFound_ReturnsNull()
    {
        var repository = new Mock<IStudentRepository>();
        var service = new StudentService(repository.Object);

        var result = await service.ChangeStatusAsync(
            Guid.NewGuid(),
            StudentStatus.Archived);

        Assert.Null(result);

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ChangeStatus_InvalidStatus_ThrowsException()
    {
        var student = new Student();
        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByIdAsync(
                student.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var service = new StudentService(repository.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.ChangeStatusAsync(
                student.Id,
                (StudentStatus)999));

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ChangeStatus_SameStatus_DoesNotSave()
    {
        var student = new Student
        {
            Status = StudentStatus.Active
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByIdAsync(
                student.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var service = new StudentService(repository.Object);

        var result = await service.ChangeStatusAsync(
            student.Id,
            StudentStatus.Active);

        Assert.NotNull(result);
        Assert.Equal(StudentStatus.Active, result.Status);

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
