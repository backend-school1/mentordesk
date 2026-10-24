using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

public record GetCardsCommand(Guid deckId) : IRequest<Result<List<CardResponse>>>;
