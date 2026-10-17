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
}
