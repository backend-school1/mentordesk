using FluentValidation;
using MentorDesk.Application.Features.Decks.UpdateDeck;

namespace MentorDesk.Application.DTOs.Validators;

public class UpdateDeckRequestValidator : AbstractValidator<UpdateDeckCommand>
{
    public UpdateDeckRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название колоды не должно быть пустым.");
    }
}
