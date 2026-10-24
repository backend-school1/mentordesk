using FluentValidation;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;
using MediatR;

public class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, Result<DeckResponse>>
{
    private readonly IDeckRepository _deckRepository;

    public CreateDeckCommandHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<Result<DeckResponse>> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.CreateDeckAsync(request.Title.Trim(), cancellationToken);

        var response = new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        };

        return Result<DeckResponse>.Success(response);
    }
}
