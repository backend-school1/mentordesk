using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Features.Decks.GetByIdDeck;

public record GetByIdQuery(Guid id) : IRequest<Result<DeckResponse>>;