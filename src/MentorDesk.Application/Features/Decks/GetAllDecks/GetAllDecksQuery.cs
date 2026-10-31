using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Features.Decks.GetAllDecks;

public record GetAllDecksQuery : IRequest<Result<List<DeckResponse>>>;
