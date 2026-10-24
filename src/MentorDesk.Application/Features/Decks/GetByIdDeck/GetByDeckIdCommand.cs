using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record GetByIdCommand(Guid id) : IRequest<Result<DeckResponse>>;
