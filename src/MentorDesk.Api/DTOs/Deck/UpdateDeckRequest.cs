namespace MentorDesk.Api.DTOs.Deck;

public record UpdateDeckRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
