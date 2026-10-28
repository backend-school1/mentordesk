using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public record DeleteDeckCommand(Guid id) : IRequest<Result>;
