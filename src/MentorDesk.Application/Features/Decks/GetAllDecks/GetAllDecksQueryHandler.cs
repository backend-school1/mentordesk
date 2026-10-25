using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public class GetAllDecksQueryHandler : IRequestHandler<GetAllDecksQuery, Result<List<DeckResponse>>>
{
    private readonly IDeckRepository _deckRepository;

    public GetAllDecksQueryHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<Result<List<DeckResponse>>> Handle(GetAllDecksQuery request, CancellationToken cancellationToken)
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
}
