using MentorDesk.Application.Common;
using MentorDesk.Application.DTOs.Card;

namespace MentorDesk.Application.Service;

public interface ICardService
{
    Task<Result<CardResponse>> UpdateCardAsync(Guid id, UpdateCardRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteCardAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<CardResponse>> CreateCardAsync(Guid deckId, CreateCardRequest request, CancellationToken cancellationToken);
    Task<Result<List<CardResponse>>> GetCardsAsync(Guid deckId, CancellationToken cancellationToken);
}
