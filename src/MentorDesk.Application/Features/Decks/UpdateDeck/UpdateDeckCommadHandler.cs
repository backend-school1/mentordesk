using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Features.Decks.UpdateDeck;

public class UpdateDeckCommandHandler : IRequestHandler<UpdateDeckCommand, Result<DeckResponse>>
{
    private readonly IDeckRepository _deckRepository;

    public UpdateDeckCommandHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<Result<DeckResponse>> Handle(UpdateDeckCommand request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(request.id, cancellationToken);

        if (deck is null)
        {
            return Result<DeckResponse>.Failure("Колода не найдена.");
        }

        deck.Title = request.Title.Trim();
        deck.Description = request.Description?.Trim();

        await _deckRepository.UpdateDeckAsync(deck, cancellationToken);

        var response = new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title,
            Description = deck.Description,
        };

        return Result<DeckResponse>.Success(response);
    }
}