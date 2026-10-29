
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Students.Create;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;
using Moq;

namespace MentorDesk.Tests;

public sealed class CreateStudentCommandHandlerTests
{
    [Fact]
    public async Task Create_NewStudent_SavesChanges()
    {
        // Arrange
        var repository = new Mock<IStudentRepository>();

        var handler = new CreateStudentsCommandHandler(
            repository.Object);

        var command = new CreateStudentCommand("@TestStudent123");

        // Act
        var result = await handler.Handle(
            command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("teststudent123", result.TelegramUserName);

        repository.Verify(
            r => r.AddAsync(
                It.Is<Student>(s =>
                    s.TelegramUserName == "teststudent123"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_ExistingStudent_ReturnsNull()
    {
        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.ExistsByUsernameAsync(
                "existinguser",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateStudentsCommandHandler(
            repository.Object);

        var command = new CreateStudentCommand("@ExistingUser");

        var result = await handler.Handle(
            command, CancellationToken.None);

        Assert.Null(result);

        repository.Verify(
            r => r.AddAsync(
                It.IsAny<Student>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_InvalidUsername_ThrowsException()
    {
        var repository = new Mock<IStudentRepository>();

        var handler = new CreateStudentsCommandHandler(
            repository.Object);

        var command = new CreateStudentCommand("abc");

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(
                command, CancellationToken.None));

        repository.Verify(
            r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
