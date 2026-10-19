namespace MentorDesk.Api.DTOs.Card;

public record UpdateCardRequest
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
