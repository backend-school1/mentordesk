using FluentValidation;
using MentorDesk.Application.DTOs.Card;

public class CreateCardRequestValidator : AbstractValidator<CreateCardCommand>
{
    public CreateCardRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Вопрос не должен быть пустым.");
        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Ответ не должен быть пустым.");
    }
}
