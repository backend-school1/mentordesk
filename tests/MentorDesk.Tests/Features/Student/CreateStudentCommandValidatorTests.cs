using MentorDesk.Application.Features.Students.Create;

namespace MentorDesk.Tests.Features.Student;

public sealed class CreateStudentCommandValidatorTests
{
    private readonly CreateStudentCommandValidator _validator = new();

    [Theory]
    [InlineData("student123")]
    [InlineData("@Student123")]
    [InlineData("student_name")]
    [InlineData("  @Student123  ")]
    public void ValidUsername_Passes(string username)
    {
        var command = new CreateStudentCommand(username);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("@@student123")]
    [InlineData("student!123")]
    [InlineData("abcdefghijklmnopqrstuvwxyz1234567")]
    public void InvalidUsername_Fails(string username)
    {
        var command = new CreateStudentCommand(username);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
    }
}
