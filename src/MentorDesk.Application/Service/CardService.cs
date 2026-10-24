using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Service;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IDeckRepository _deckRepository;

    public CardService(ICardRepository cardRepository, IDeckRepository deckRepository)
    {
        _cardRepository = cardRepository;
        _deckRepository = deckRepository;
    }

    public async Task<Result<CardResponse>> UpdateCardAsync(Guid id, UpdateCardRequest request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetCardAsync(id, cancellationToken);

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

    public async Task<Result<bool>> DeleteCardAsync(Guid id, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetCardAsync(id, cancellationToken);

        if (card is null)
        {
            return Result<bool>.Failure("Карточка не найдена.");
        }

        _cardRepository.RemoveCard(card);

        await _cardRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<CardResponse>> CreateCardAsync(Guid deckId, CreateCardRequest request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(deckId, cancellationToken);

        if (deck is null)
        {
            return Result<CardResponse>.Failure("Колода не найдена.");
        }

        var card = await _cardRepository.CreateCardAsync(deckId, request.Question.Trim(), request.Answer.Trim(), cancellationToken);

        var response = new CardResponse
        {
            Id = card.Id,
            DeckId = card.DeckId,
            Question = card.Question,
            Answer = card.Answer
        };

        return Result<CardResponse>.Success(response);
    }

    public async Task<Result<List<CardResponse>>> GetCardsAsync(Guid deckId, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(deckId, cancellationToken);

        if (deck is null)
        {
            return Result<List<CardResponse>>.Failure("Колода не найдена.");
        }

        var cards = await _cardRepository.GetCardsByDeckAsync(deckId, cancellationToken);

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
