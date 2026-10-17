using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Abstractions.Repositories;

// Я не делал UnitOfWork, в интернетах пишут, что абстракция над абстракцией лишнее ¯\_(ツ)_/¯
public interface IDeckRepository
{
    Task<Deck?> GetDeckAsync(Guid deckId, CancellationToken cancellationToken);
}
