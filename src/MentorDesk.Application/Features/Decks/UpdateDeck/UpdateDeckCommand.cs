using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Features.Decks.UpdateDeck;

public record UpdateDeckCommand(Guid id, string Title, string? Description) : IRequest<Result<DeckResponse>>;