using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

namespace MentorDesk.Application.Features.Cards.CreateCard;

public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, Result<CardResponse>>
{
    private readonly ICardRepository _cardRepository;
    private readonly IDeckRepository _deckRepository;

    public CreateCardCommandHandler(ICardRepository cardRepository, IDeckRepository deckRepository)
    {
        _cardRepository = cardRepository;
        _deckRepository = deckRepository;
    }

    public async Task<Result<CardResponse>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(request.deckId, cancellationToken);

        if (deck is null)
        {
            return Result<CardResponse>.Failure("Колода не найдена.");
        }

        var card = await _cardRepository.CreateCardAsync(request.deckId, request.Question.Trim(), request.Answer.Trim(), cancellationToken);

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