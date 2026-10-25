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
}
