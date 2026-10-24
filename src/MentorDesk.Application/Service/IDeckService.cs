using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Deck;

namespace MentorDesk.Application.Service;

public interface IDeckService
{
    Task<Result<List<DeckResponse>>> GetAllDecksAsync(CancellationToken cancellationToken);
    Task<Result<DeckResponse>> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken);
    Task<Result<DeckResponse>> GetByDeckIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result> DeleteDeckAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<DeckResponse>> UpdateDeckAsync(Guid id, UpdateDeckRequest request, CancellationToken cancellationToken);
}
