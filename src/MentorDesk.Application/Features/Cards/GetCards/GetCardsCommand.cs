using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;
using MediatR;

public record GetCardsCommand(Guid deckId) : IRequest<Result<List<CardResponse>>>;
