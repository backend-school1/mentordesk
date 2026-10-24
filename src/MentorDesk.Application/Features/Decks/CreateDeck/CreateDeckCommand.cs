using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record CreateDeckCommand(string Title) : IRequest<Result<DeckResponse>>;
