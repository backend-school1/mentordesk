using MentorDesk.Application.Features.Students.ChangeStatus;
using MentorDesk.Domain.Enums;

namespace MentorDesk.Tests.Features.Student;

public sealed class ChangeStudentStatusCommandValidatorTests
{
    private readonly ChangeStudentStatusCommandValidator
        _validator = new();

    [Fact]
    public async Task InvalidStatus_ReturnsValidationError()
    {
        var command = new ChangeStudentStatusCommand(
            Guid.NewGuid(),
            (StudentStatus)999);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.PropertyName == "Status");
    }

    [Fact]
    public async Task ValidStatus_PassesValidation()
    {
        var command = new ChangeStudentStatusCommand(
            Guid.NewGuid(),
            StudentStatus.Active);

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task EmptyStudentId_ReturnsValidationError()
    {
        var command = new ChangeStudentStatusCommand(
            Guid.Empty,
            StudentStatus.Archived);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors,
            e => e.PropertyName == "StudentId");
    }
}
