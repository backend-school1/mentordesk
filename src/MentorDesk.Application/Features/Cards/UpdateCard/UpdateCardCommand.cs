using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;
using MediatR;

public record UpdateCardCommand(Guid id, string Question, string Answer) : IRequest<Result<CardResponse>>;
