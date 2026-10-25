using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, Result<DeckResponse>>
{
    private readonly IDeckRepository _deckRepository;

    public GetByIdQueryHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<Result<DeckResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(request.id, cancellationToken);

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
}
