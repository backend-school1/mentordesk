using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Students.GetAll;
using Moq;

namespace MentorDesk.Tests.Features.Student;

public sealed class GetStudentsQueryHandlerTests
{
    [Fact]
    public async Task GetAll_ReturnsStudents()
    {
        // Arrange
        var students = new List<MentorDesk.Domain.Entities.Student>
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

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(2, result.Value.Count);

        Assert.Contains(result.Value, s =>
            s.TelegramUserName == "student1");

        Assert.Contains(result.Value, s =>
            s.TelegramUserName == "student2");

        repository.Verify(
            r => r.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
