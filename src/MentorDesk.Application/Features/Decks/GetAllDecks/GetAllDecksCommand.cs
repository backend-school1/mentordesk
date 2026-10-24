using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;
using MediatR;

public record GetAllDecksCommand : IRequest<Result<List<DeckResponse>>>;
