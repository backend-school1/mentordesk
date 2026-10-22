using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.DTOs.Card;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Service;

public class DeckService : IDeckService
{
    private readonly IDeckRepository _deckRepository;

    public DeckService(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<List<DeckResponse>> GetAllDecksAsync(CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetAllDecksAsync(cancellationToken);

        return deck
            .Select(deck => new DeckResponse
            {
                Id = deck.Id,
                Title = deck.Title
            })
            .ToList();
    }

    public async Task<DeckResponse> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "Название колоды не должно быть пустым.",
                nameof(request.Title));
        }

        var deck = await _deckRepository.CreateDeckAsync(request.Title.Trim(), cancellationToken);

        return new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };
    }

    public async Task<DeckResponse?> GetByDeckIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(id, cancellationToken);
        if (deck is null)
        {
            return null;
        }

        return new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };
    }

    public async Task<bool> DeleteDeckAsync(Guid id, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(id, cancellationToken);

        if (deck is null)
        {
            return false;
        }

        await _deckRepository.DeleteDeckAsync(deck, cancellationToken);

        return true;
    }

    public async Task<DeckResponse?> UpdateDeckAsync(Guid id, UpdateDeckRequest request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(id, cancellationToken);

        if (deck is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "Название колоды не должно быть пустым.",
                nameof(request.Title));
        }

        deck.Title = request.Title.Trim();

        await _deckRepository.UpdateDeckAsync(
            deck,
            cancellationToken);

        return new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };
    }
}
