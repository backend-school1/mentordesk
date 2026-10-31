using FluentValidation;
using MentorDesk.Application.Features.Cards.CreateCard;

namespace MentorDesk.Application.DTOs.Validators;

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
