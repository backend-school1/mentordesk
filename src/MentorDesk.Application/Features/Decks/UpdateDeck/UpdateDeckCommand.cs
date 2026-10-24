using FluentValidation;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;
using MediatR;

public record UpdateDeckCommand(Guid id, string Title) : IRequest<Result<DeckResponse>>;
