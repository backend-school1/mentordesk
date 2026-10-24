using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;
using MediatR;

public record DeleteCardCommand(Guid id) : IRequest<Result<bool>>;
