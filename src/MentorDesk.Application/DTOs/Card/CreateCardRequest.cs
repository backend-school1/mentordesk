namespace MentorDesk.Application.DTOs.Card;

public record CreateCardRequest
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
