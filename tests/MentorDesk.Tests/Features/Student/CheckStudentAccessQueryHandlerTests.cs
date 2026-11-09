
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Students.Access;
using MentorDesk.Domain.Enums;
using Moq;
using StudentEntity = MentorDesk.Domain.Entities.Student;

namespace MentorDesk.Tests.Features.Student;

public sealed class CheckStudentAccessQueryHandlerTests
{
    [Fact]
    public async Task ActiveStudent_WithMatchingTelegramId_ReturnsAllowed()
    {
        // Arrange
        const long telegramId = 123;

        var student = new StudentEntity
        {
            TelegramId = telegramId,
            TelegramUserName = "student1",
            Status = StudentStatus.Active
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByTelegramIdAsync(
                telegramId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var handler = new CheckStudentAccessQueryHandler(
            repository.Object);

        // Act
        var result = await handler.Handle(
            new CheckStudentAccessQuery(telegramId, "student1"),
            CancellationToken.None);

        // Assert
        Assert.Equal(StudentAccessStatus.Allowed, result.Status);
        Assert.Equal(student.Id, result.StudentId);

        repository.Verify(
            r => r.GetByUsernameAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task ArchivedStudent_WithMatchingTelegramId_ReturnsDenied()
    {
        // Arrange
        const long telegramId = 123;

        var student = new StudentEntity
        {
            TelegramId = telegramId,
            TelegramUserName = "student1",
            Status = StudentStatus.Archived
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByTelegramIdAsync(
                telegramId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var handler = new CheckStudentAccessQueryHandler(
            repository.Object);

        // Проверяем исходные данные
        Assert.Equal(StudentStatus.Archived, student.Status);

        // Act
        var result = await handler.Handle(
            new CheckStudentAccessQuery(telegramId, "student1"),
            CancellationToken.None);

        // Assert
        Assert.Equal(StudentAccessStatus.Denied, result.Status);
        Assert.Null(result.StudentId);
    }


    [Fact]
    public async Task UnknownStudent_ReturnsDenied()
    {
        // Arrange
        const long telegramId = 999;

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByTelegramIdAsync(
                telegramId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentEntity?)null);

        repository
            .Setup(r => r.GetByUsernameAsync(
                "unknown",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentEntity?)null);

        var handler = new CheckStudentAccessQueryHandler(
            repository.Object);

        // Act
        var result = await handler.Handle(
            new CheckStudentAccessQuery(telegramId, "unknown"),
            CancellationToken.None);

        // Assert
        Assert.Equal(StudentAccessStatus.Denied, result.Status);
        Assert.Null(result.StudentId);
    }


    [Fact]
    public async Task ActiveStudent_WithoutTelegramId_ReturnsRequiresBinding()
    {
        // Arrange
        const long telegramId = 123;

        var student = new StudentEntity
        {
            TelegramId = null,
            TelegramUserName = "student1",
            Status = StudentStatus.Active
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByTelegramIdAsync(
                telegramId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentEntity?)null);

        repository
            .Setup(r => r.GetByUsernameAsync(
                "student1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var handler = new CheckStudentAccessQueryHandler(
            repository.Object);

        Assert.Equal(StudentStatus.Active, student.Status);
        Assert.Null(student.TelegramId);

        // Act
        var result = await handler.Handle(
            new CheckStudentAccessQuery(telegramId, "@Student1"),
            CancellationToken.None);

        // Assert
        Assert.Equal(
            StudentAccessStatus.RequiresBinding,
            result.Status);

        Assert.Equal(student.Id, result.StudentId);

        repository.Verify(
            r => r.GetByUsernameAsync(
                "student1",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task ArchivedStudent_FoundByUsername_ReturnsDenied()
    {
        const long telegramId = 999;
        // Arrange
        var student = new StudentEntity
        {
            TelegramId = telegramId,
            TelegramUserName = "student1",
            Status = StudentStatus.Active
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByUsernameAsync(
                "student1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var handler = new CheckStudentAccessQueryHandler(
            repository.Object);

        // Act
        var result = await handler.Handle(
            new CheckStudentAccessQuery(999, "student1"),
            CancellationToken.None);

        // Assert
        Assert.Equal(StudentAccessStatus.Denied, result.Status);
        Assert.Null(result.StudentId);
    }

    [Fact]
    public async Task UsernameBoundToAnotherTelegramId_ReturnsDenied()
    {
        const long telegramId = 911;
        // Arrange
        // const long existingTelegramId = 123;
        const long incomingTelegramId = 999;

        var student = new StudentEntity
        {
            TelegramId = telegramId,
            TelegramUserName = "student1",
            Status = StudentStatus.Active
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetByUsernameAsync(
                "student1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var handler = new CheckStudentAccessQueryHandler(
            repository.Object);

        // Act
        var result = await handler.Handle(
            new CheckStudentAccessQuery(
                incomingTelegramId,
                "student1"),
            CancellationToken.None);

        // Assert
        Assert.Equal(StudentAccessStatus.Denied, result.Status);
        Assert.Null(result.StudentId);
    }

    [Fact]
    public async Task UnknownStudent_WithoutUsername_ReturnsDenied()
    {
        // Arrange
        var repository = new Mock<IStudentRepository>();

        var handler = new CheckStudentAccessQueryHandler(
            repository.Object);

        // Act
        var result = await handler.Handle(
            new CheckStudentAccessQuery(999, string.Empty),
            CancellationToken.None);

        // Assert
        Assert.Equal(StudentAccessStatus.Denied, result.Status);
        Assert.Null(result.StudentId);

        repository.Verify(
            r => r.GetByUsernameAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
