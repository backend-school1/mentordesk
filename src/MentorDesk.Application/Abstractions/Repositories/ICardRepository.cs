using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Abstractions.Repositories;

public interface ICardRepository
{
    Task<IReadOnlyList<Card>> GetCardsByDeckAsync(Guid deckId, CancellationToken cancellationToken);
    Task AddCards(IEnumerable<Card> cards, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<Card?> GetCardAsync(Guid id,CancellationToken cancellationToken);
    void RemoveCard(Card card);
    Task<Card> CreateCardAsync(Guid deckId, string question, string answer, CancellationToken cancellationToken);
}
