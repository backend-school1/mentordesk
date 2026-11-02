using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Features.Decks.CreateDeck;

public record CreateDeckCommand(string Title, string? Description) : IRequest<Result<DeckResponse>>;
