namespace MentorDesk.Application.DTOs.Deck;

public record CreateDeckRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
