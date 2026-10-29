using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MentorDesk.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _context;

    public CardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Card>> GetCardsByDeckAsync(Guid deckId, CancellationToken cancellationToken)
    {
        return await _context.Cards.Where(x => x.DeckId == deckId).ToListAsync(cancellationToken);
    }

    public async Task AddCards(IEnumerable<Card> cards, CancellationToken cancellationToken)
    {
        await _context.Cards.AddRangeAsync(cards, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Card?> GetCardAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Cards.FirstOrDefaultAsync(card => card.Id == id, cancellationToken);
    }

    public void RemoveCard(Card card)
    {
        _context.Cards.Remove(card);
    }

    public async Task<Card> CreateCardAsync(Guid deckId, string question, string answer, CancellationToken cancellationToken)
    {
        var card = new Card
        {
            Id = Guid.NewGuid(),
            DeckId = deckId,
            Question = question,
            Answer = answer
        };

        _context.Cards.Add(card);

        await _context.SaveChangesAsync(cancellationToken);

        return card;
    }
}
