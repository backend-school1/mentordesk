using FluentValidation;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;
using MediatR;

public class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand, Result<bool>>
{
    private readonly ICardRepository _cardRepository;

    public DeleteCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Result<bool>> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetCardAsync(request.id, cancellationToken);

        if (card is null)
        {
            return Result<bool>.Failure("Карточка не найдена.");
        }

        _cardRepository.RemoveCard(card);

        await _cardRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
