using FluentValidation;

namespace MentorDesk.Application.Features.Students.ChangeStatus;

public sealed class ChangeStudentStatusCommandValidator : AbstractValidator<ChangeStudentStatusCommand>
{
    public ChangeStudentStatusCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid student status.");
    }
}
