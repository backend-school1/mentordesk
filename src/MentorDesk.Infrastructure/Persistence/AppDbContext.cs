using MentorDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MentorDesk.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // DbSet-ы сущностей появятся здесь (задача #1 — Deck и Card).
    public DbSet<Deck> Decks { get; set; }
    public DbSet<Card> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
