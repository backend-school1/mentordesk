using MentorDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorDesk.Infrastructure.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.HasOne<Deck>()
            .WithMany()
            .HasForeignKey(c => c.DeckId);

        builder.Property(x => x.Question)
            .IsRequired();

        builder.Property(x => x.Answer)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
