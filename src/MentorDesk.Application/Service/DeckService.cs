using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
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

    public async Task<Result<List<DeckResponse>>> GetAllDecksAsync(CancellationToken cancellationToken)
    {
        var decks = await _deckRepository.GetAllDecksAsync(cancellationToken);

        var response = decks
            .Select(deck => new DeckResponse
            {
                Id = deck.Id,
                Title = deck.Title
            })
            .ToList();

        return Result<List<DeckResponse>>.Success(response);
    }

    public async Task<Result<DeckResponse>> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.CreateDeckAsync(request.Title.Trim(), cancellationToken);

        var response = new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };

        return Result<DeckResponse>.Success(response);
    }

    public async Task<Result<DeckResponse>> GetByDeckIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(id, cancellationToken);

        if (deck is null)
        {
            return Result<DeckResponse>.Failure("Колода не найдена.");
        }

        var response = new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };

        return Result<DeckResponse>.Success(response);
    }

    public async Task<Result> DeleteDeckAsync(Guid id, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(id, cancellationToken);

        if (deck is null)
        {
            return Result.Failure("Колода не найдена.");
        }

        await _deckRepository.DeleteDeckAsync(deck, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<DeckResponse>> UpdateDeckAsync(Guid id, UpdateDeckRequest request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(id, cancellationToken);

        if (deck is null)
        {
            return Result<DeckResponse>.Failure("Колода не найдена.");
        }

        deck.Title = request.Title.Trim();

        await _deckRepository.UpdateDeckAsync(deck, cancellationToken);

        var response = new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };

        return Result<DeckResponse>.Success(response);
    }
}
