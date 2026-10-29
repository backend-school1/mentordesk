using FluentValidation;
using MentorDesk.Application.DTOs.Deck;

public class UpdateDeckRequestValidator : AbstractValidator<UpdateDeckCommand>
{
    public UpdateDeckRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название колоды не должно быть пустым.");
    }
}
