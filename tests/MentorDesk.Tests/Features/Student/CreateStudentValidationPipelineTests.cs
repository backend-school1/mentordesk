using FluentValidation;
using MediatR;
using MentorDesk.Application;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Students.Create;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MentorDesk.Tests.Features.Student;

public sealed class CreateStudentValidationPipelineTests
{
    [Fact]
    public async Task InvalidUsername_IsRejectedByPipeline()
    {
        // Arrange
        var repository = new Mock<IStudentRepository>();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplication();
        services.AddSingleton(repository.Object);

        await using var provider =
            services.BuildServiceProvider();

        var sender = provider.GetRequiredService<ISender>();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => sender.Send(
                new CreateStudentCommand("abc")));

        repository.Verify(
            r => r.AddAsync(
                It.IsAny<MentorDesk.Domain.Entities.Student>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
