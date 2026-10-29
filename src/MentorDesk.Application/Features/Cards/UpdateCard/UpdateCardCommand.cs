using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

namespace MentorDesk.Application.Features.Cards.UpdateCard;

public record UpdateCardCommand(Guid id, string Question, string Answer) : IRequest<Result<CardResponse>>;