using FluentValidation;

namespace MentorDesk.Application.Features.Decks.Import;

public class ImportDeckCommandValidator : AbstractValidator<ImportDeckCommand>
{
    public ImportDeckCommandValidator()
    {
        RuleFor(x => x.DeckId)
            .NotEmpty()
            .WithMessage("ID колоды не может быть пустым");

        RuleFor(x => x.Csv)
            .NotNull()
            .WithMessage("CSV файл не может быть пустым");
    }
}
