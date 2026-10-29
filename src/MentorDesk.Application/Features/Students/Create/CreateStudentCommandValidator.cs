using FluentValidation;

namespace MentorDesk.Application.Features.Students.Create;

public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.TelegramUsername)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Matches(@"^\s*@?[A-Za-z0-9_]{5,32}\s*$")
            .WithMessage("Invalid Telegram username.");
    }
}
