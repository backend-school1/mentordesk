using MediatR;
using MentorDesk.Application.Common;

namespace MentorDesk.Application.Features.Decks.Import;

public record ImportDeckCommand(Guid DeckId, Stream Csv) : IRequest<Result<ImportResult>>;

