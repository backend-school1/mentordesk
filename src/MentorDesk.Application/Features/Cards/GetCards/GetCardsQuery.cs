using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

namespace MentorDesk.Application.Features.Cards.GetCards;

public record GetCardsQuery(Guid deckId) : IRequest<Result<List<CardResponse>>>;
