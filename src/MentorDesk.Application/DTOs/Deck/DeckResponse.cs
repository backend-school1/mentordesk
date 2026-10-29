namespace MentorDesk.Application.DTOs.Deck;

public record DeckResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
