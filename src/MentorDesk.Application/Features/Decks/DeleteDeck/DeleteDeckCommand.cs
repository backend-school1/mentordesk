using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;
using MediatR;

public record DeleteDeckCommand(Guid id) : IRequest<Result>;
