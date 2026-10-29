using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

namespace MentorDesk.Application.Features.Cards.CreateCard;

public record CreateCardCommand(Guid deckId, string Question, string Answer) : IRequest<Result<CardResponse>>;