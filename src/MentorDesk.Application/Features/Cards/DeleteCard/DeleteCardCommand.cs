using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

public record DeleteCardCommand(Guid id) : IRequest<Result<bool>>;
