using FluentValidation;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;
using MediatR;

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

        await _deckRepository.UpdateDeckAsync(deck, cancellationToken);

        var response = new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };

        return Result<DeckResponse>.Success(response);
    }
}
