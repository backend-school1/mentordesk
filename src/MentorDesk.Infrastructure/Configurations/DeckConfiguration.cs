using MentorDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorDesk.Infrastructure.Configurations;

public class DeckConfiguration : IEntityTypeConfiguration<Deck>
{
    public void Configure(EntityTypeBuilder<Deck> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired();

        builder.Property(x => x.Description);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
