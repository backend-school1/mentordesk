using MentorDesk.Application.DTOs.Card;
using MentorDesk.Application.Abstractions.Repositories;

namespace MentorDesk.Application.Service;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;

    public CardService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
    public async Task<CardResponse?> UpdateCardAsync(Guid id, UpdateCardRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new ArgumentException(
                "Вопрос карточки не должен быть пустым.",
                nameof(request.Question));
        }

        if (string.IsNullOrWhiteSpace(request.Answer))
        {
            throw new ArgumentException(
                "Ответ карточки не должен быть пустым.",
                nameof(request.Answer));
        }

        var card = await _cardRepository.GetCardAsync(id, cancellationToken);

        if (card is null)
        {
            return null;
        }

        card.Question = request.Question.Trim();
        card.Answer = request.Answer.Trim();

        await _cardRepository.SaveChangesAsync(cancellationToken);

        return new CardResponse
        {
            Id = card.Id,
            DeckId = card.DeckId,
            Question = card.Question,
            Answer = card.Answer
        };
    }

    public async Task<bool> DeleteCardAsync(Guid id, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetCardAsync(id, cancellationToken);

        if (card is null)
        {
            return false;
        }

        _cardRepository.RemoveCard(card);

        await _cardRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<CardResponse> CreateCardAsync(Guid deckId, CreateCardRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new ArgumentException(
                "Вопрос карточки не должен быть пустым.",
                nameof(request.Question));
        }

        if (string.IsNullOrWhiteSpace(request.Answer))
        {
            throw new ArgumentException(
                "Ответ карточки не должен быть пустым.",
                nameof(request.Answer));
        }

        var card = await _cardRepository.CreateCardAsync(deckId, request.Question.Trim(), request.Answer.Trim(), cancellationToken);

        return new CardResponse
        {
            Id = card.Id,
            DeckId = card.DeckId,
            Question = card.Question,
            Answer = card.Answer
        };
    }

    public async Task<List<CardResponse>> GetCardsAsync(Guid deckId, CancellationToken cancellationToken)
    {
        var cards = await _cardRepository.GetCardsByDeckAsync(deckId, cancellationToken);

        return cards.Select(card => new CardResponse
        {
            Id = card.Id,
            Question = card.Question,
            Answer = card.Answer
        }).ToList();
    }
}
