using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

public class GetCardsCommandHandler : IRequestHandler<GetCardsCommand, Result<List<CardResponse>>>
{
    private readonly ICardRepository _cardRepository;
    private readonly IDeckRepository _deckRepository;

    public GetCardsCommandHandler(ICardRepository cardRepository, IDeckRepository deckRepository)
    {
        _cardRepository = cardRepository;
        _deckRepository = deckRepository;
    }

    public async Task<Result<List<CardResponse>>> Handle(GetCardsCommand request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(request.deckId, cancellationToken);

        if (deck is null)
        {
            return Result<List<CardResponse>>.Failure("Колода не найдена.");
        }

        var cards = await _cardRepository.GetCardsByDeckAsync(request.deckId, cancellationToken);

        var response = cards.Select(card => new CardResponse
        {
            Id = card.Id,
            Question = card.Question,
            Answer = card.Answer,
            DeckId = card.DeckId
        }).ToList();

        return Result<List<CardResponse>>.Success(response);
    }
}
