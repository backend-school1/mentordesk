using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Abstractions.Repositories;

public interface IDeckRepository
{
    Task<Deck?> GetDeckAsync(Guid deckId, CancellationToken cancellationToken);
}
