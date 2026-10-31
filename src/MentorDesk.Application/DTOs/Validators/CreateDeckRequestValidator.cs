using FluentValidation;
using MentorDesk.Application.Features.Decks.CreateDeck;

namespace MentorDesk.Application.DTOs.Validators;

public class CreateDeckRequestValidator : AbstractValidator<CreateDeckCommand>
{
    public CreateDeckRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название колоды не должно быть пустым.");
    }
}
