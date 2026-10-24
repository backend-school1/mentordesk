using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

public record CreateCardCommand(Guid deckId, string Question, string Answer) : IRequest<Result<CardResponse>>;
