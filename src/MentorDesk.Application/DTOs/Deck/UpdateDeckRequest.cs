using System.ComponentModel.DataAnnotations;

namespace MentorDesk.Application.DTOs.Deck;

public record UpdateDeckRequest
{
    [Required(ErrorMessage = "Название колоды не должно быть пустым.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
