using FluentValidation;
using MentorDesk.Application.DTOs.Card;

public class CreateCardRequestValidator : AbstractValidator<CreateCardRequest>
{
    public CreateCardRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Вопрос не должен быть пустым.");
        
        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Вопрос не должен быть пустым.");
    }
}
