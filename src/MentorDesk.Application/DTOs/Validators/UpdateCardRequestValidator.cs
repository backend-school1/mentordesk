using FluentValidation;
using MentorDesk.Application.Features.Cards.UpdateCard;

namespace MentorDesk.Application.DTOs.Validators;

public class UpdateCardRequestValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Вопрос не должен быть пустым.");

        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Ответ не должен быть пустым.");
    }
}
