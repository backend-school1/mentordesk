using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

public record UpdateCardCommand(Guid id, string Question, string Answer) : IRequest<Result<CardResponse>>;
