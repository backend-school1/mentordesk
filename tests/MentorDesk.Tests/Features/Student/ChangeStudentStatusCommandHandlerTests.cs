using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Students.ChangeStatus;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;
using Moq;

namespace MentorDesk.Tests;

public sealed class ChangeStudentStatusCommandHandlerTests
{
    [Fact]
    public async Task ActiveToArchived_SavesChanges()
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

        var handler = new ChangeStudentStatusCommandHandler(
            repository.Object);

        var command = new ChangeStudentStatusCommand(
            student.Id,
            StudentStatus.Archived);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StudentStatus.Archived, result.Status);

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task StudentNotFound_ReturnsNull()
    {
        var repository = new Mock<IStudentRepository>();
        var handler = new ChangeStudentStatusCommandHandler(
            repository.Object);

        var command = new ChangeStudentStatusCommand(
            Guid.NewGuid(), StudentStatus.Archived);

        var result = await handler.Handle(
            command, CancellationToken.None);

        Assert.Null(result);

        repository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SameStatus_DoesNotSave()
    {
        var student = new Student
        {
            Status = StudentStatus.Active
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByIdAsync(
                student.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var handler = new ChangeStudentStatusCommandHandler(
            repository.Object);

        var command = new ChangeStudentStatusCommand(
            student.Id, StudentStatus.Active);

        var result = await handler.Handle(
            command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(StudentStatus.Active, result.Status);

        repository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task InvalidStatus_ThrowsException()
    {
        var repository = new Mock<IStudentRepository>();

        var handler = new ChangeStudentStatusCommandHandler(
            repository.Object);

        var command = new ChangeStudentStatusCommand(
            Guid.NewGuid(), (StudentStatus)999);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => handler.Handle(
                command, CancellationToken.None));

        repository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

}
