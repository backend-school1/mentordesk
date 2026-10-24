using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record UpdateDeckCommand(Guid id, string Title) : IRequest<Result<DeckResponse>>;
