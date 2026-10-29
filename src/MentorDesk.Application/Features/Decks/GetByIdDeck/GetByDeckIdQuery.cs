using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record GetByIdQuery(Guid id) : IRequest<Result<DeckResponse>>;
