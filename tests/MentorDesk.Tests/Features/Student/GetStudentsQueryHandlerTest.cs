using MentorDesk.Application.Features.Students.GetAll;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;
using Moq;

namespace MentorDesk.Tests;

public sealed class GetStudentsQueryHandlerTests
{
    [Fact]
    public async Task GetAll_ReturnsStudents()
    {
        // Arrange
        var students = new List<Student>
        {
            new() { TelegramUserName = "student1" },
            new() { TelegramUserName = "student2" }
        };

        var repository = new Mock<IStudentRepository>();

        repository
            .Setup(r => r.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        var handler = new GetStudentsQueryHandler(
            repository.Object);

        // Act
        var result = await handler.Handle(
            new GetStudentsQuery(),
            CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s =>
            s.TelegramUserName == "student1");
        Assert.Contains(result, s =>
            s.TelegramUserName == "student2");

        repository.Verify(
            r => r.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
