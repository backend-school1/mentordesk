using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;
using MediatR;

public record CreateDeckCommand(string Title) : IRequest<Result<DeckResponse>>;
