namespace MentorDesk.Application.DTOs.Card;

public record CardResponse
{
    public Guid Id { get; set; }
    public Guid DeckId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
