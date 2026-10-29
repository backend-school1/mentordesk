using MediatR;
using MentorDesk.Application.Common;

namespace MentorDesk.Application.Features.Cards.DeleteCard;

public record DeleteCardCommand(Guid id) : IRequest<Result<bool>>;