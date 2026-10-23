using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Service;

public interface IDeckService
{
    Task<List<DeckResponse>> GetAllDecksAsync(CancellationToken cancellationToken);
    Task<DeckResponse> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken);
    Task<DeckResponse?> GetByDeckIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> DeleteDeckAsync(Guid id, CancellationToken cancellationToken);
    Task<DeckResponse?> UpdateDeckAsync(Guid id, UpdateDeckRequest request, CancellationToken cancellationToken);
}
