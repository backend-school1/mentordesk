using FluentValidation;
using MentorDesk.Application.DTOs.Deck;

public class CreateDeckRequestValidator : AbstractValidator<CreateDeckRequest>
{
	public CreateDeckRequestValidator()
	{
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название колоды не должно быть пустым.");
    }
}
