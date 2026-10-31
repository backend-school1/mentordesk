using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Features.Decks.CreateDeck;

public class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, Result<DeckResponse>>
{
    private readonly IDeckRepository _deckRepository;

    public CreateDeckCommandHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<Result<DeckResponse>> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.CreateDeckAsync(request.Title.Trim(), request.Description?.Trim(), cancellationToken);

        var response = new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title,
            Description = deck.Description,
        };

        return Result<DeckResponse>.Success(response);
    }
}
