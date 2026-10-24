using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;
using MediatR;

public record CreateCardCommand(Guid deckId, string Question, string Answer) : IRequest<Result<CardResponse>>;
