using System;
using MentorDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MentorDesk.Infrastructure.Configurations;
/// <summary>
/// Represents the configuration EF settings for a student in the MentorDesk system.
/// </summary>
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{

    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);


        builder.Property(s => s.TelegramUserName)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(s => s.TelegramUserName)
            .IsUnique();

        builder.Property(s => s.TelegramId)
            .IsRequired(false);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();



    }
}
