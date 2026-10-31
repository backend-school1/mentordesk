using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;

namespace MentorDesk.Application.Features.Decks.DeleteDeck;

public class DeleteDeckCommandHandler : IRequestHandler<DeleteDeckCommand, Result>
{
    private readonly IDeckRepository _deckRepository;

    public DeleteDeckCommandHandler(IDeckRepository deckRepository)
    {
        _deckRepository = deckRepository;
    }

    public async Task<Result> Handle(DeleteDeckCommand request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(request.id, cancellationToken);

        if (deck is null)
        {
            return Result.Failure("Колода не найдена.");
        }

        await _deckRepository.DeleteDeckAsync(deck, cancellationToken);

        return Result.Success();
    }
}
