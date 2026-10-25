using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record CreateDeckCommand(string Title, string? Description) : IRequest<Result<DeckResponse>>;
