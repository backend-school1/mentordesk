using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

namespace MentorDesk.Application.Features.Cards.UpdateCard;

public class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, Result<CardResponse>>
{
    private readonly ICardRepository _cardRepository;

    public UpdateCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Result<CardResponse>> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetCardAsync(request.id, cancellationToken);

        if (card is null)
        {
            return Result<CardResponse>.Failure("Карточка не найдена.");
        }

        card.Question = request.Question.Trim();
        card.Answer = request.Answer.Trim();

        await _cardRepository.SaveChangesAsync(cancellationToken);

        var response = new CardResponse
        {
            Id = card.Id,
            DeckId = card.DeckId,
            Question = card.Question,
            Answer = card.Answer
        };

        return Result<CardResponse>.Success(response);
    }
}
