using MediatR;

namespace MentorDesk.Application.Features.Decks.Import;

public record ImportDeckCommand(Guid DeckId, Stream Csv) : IRequest<ImportResult>;

