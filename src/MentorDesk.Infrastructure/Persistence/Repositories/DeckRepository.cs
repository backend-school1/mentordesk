using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MentorDesk.Infrastructure.Persistence.Repositories;

public class DeckRepository : IDeckRepository
{
    private readonly AppDbContext _context;

    public DeckRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Deck?> GetDeckAsync(Guid deckId, CancellationToken cancellationToken)
    {
        return await _context.Decks.FirstOrDefaultAsync(x => x.Id == deckId, cancellationToken);
    }

    public async Task<List<Deck>> GetAllDecksAsync(CancellationToken cancellationToken)
    {
        return await _context.Decks.ToListAsync(cancellationToken);
    }

    public async Task DeleteDeckAsync(Deck deck, CancellationToken cancellationToken)
    {
        _context.Decks.Remove(deck);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Deck> CreateDeckAsync(string title, CancellationToken cancellationToken)
    {
        var deck = new Deck
        {
            Id = Guid.NewGuid(),
            Title = title
        };

        _context.Decks.Add(deck);

        await _context.SaveChangesAsync(cancellationToken);

        return deck;
    }

    public async Task UpdateDeckAsync(Deck deck, CancellationToken cancellationToken)
    {
        _context.Decks.Update(deck);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
