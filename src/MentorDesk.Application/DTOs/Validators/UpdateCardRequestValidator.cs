using FluentValidation;
using MentorDesk.Application.DTOs.Card;

public class UpdateCardRequestValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardRequestValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Вопрос не должен быть пустым.");

        RuleFor(x => x.Answer)
            .NotEmpty().WithMessage("Вопрос не должен быть пустым.");
    }
}
