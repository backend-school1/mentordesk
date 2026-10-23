using MentorDesk.Application.DTOs.Card;

namespace MentorDesk.Application.Service;

public interface ICardService
{
    Task<CardResponse?> UpdateCardAsync(Guid id, UpdateCardRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteCardAsync(Guid id, CancellationToken cancellationToken);
    Task<CardResponse> CreateCardAsync(Guid deckId, CreateCardRequest request, CancellationToken cancellationToken);
    Task<List<CardResponse>> GetCardsAsync(Guid deckId, CancellationToken cancellationToken);
}
