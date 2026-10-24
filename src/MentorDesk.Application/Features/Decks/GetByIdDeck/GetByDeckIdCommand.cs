using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;
using MediatR;

public record GetByIdCommand(Guid id) : IRequest<Result<DeckResponse>>;
