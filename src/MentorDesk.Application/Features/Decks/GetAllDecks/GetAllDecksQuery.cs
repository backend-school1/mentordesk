using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record GetAllDecksQuery : IRequest<Result<List<DeckResponse>>>;
