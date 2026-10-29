using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Abstractions.Repositories;

public interface IDeckRepository
{
    Task<Deck?> GetDeckAsync(Guid deckId, CancellationToken cancellationToken);
    Task<List<Deck>> GetAllDecksAsync(CancellationToken cancellationToken);
    Task DeleteDeckAsync(Deck deck, CancellationToken cancellationToken);
    Task<Deck> CreateDeckAsync(string title, string? description, CancellationToken cancellationToken);
    Task UpdateDeckAsync(Deck deck, CancellationToken cancellationToken);
}
