using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record GetAllDecksCommand : IRequest<Result<List<DeckResponse>>>;
