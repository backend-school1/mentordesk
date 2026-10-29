using MediatR;
using MentorDesk.Application.Common;

namespace MentorDesk.Application.Features.Decks.DeleteDeck;

public record DeleteDeckCommand(Guid id) : IRequest<Result>;