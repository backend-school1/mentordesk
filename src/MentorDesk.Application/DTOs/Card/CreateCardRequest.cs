using System.ComponentModel.DataAnnotations;

namespace MentorDesk.Application.DTOs.Card;

public record CreateCardRequest
{
    [Required(ErrorMessage = "Вопрос не должен быть пустым.")]
    public string Question { get; set; } = string.Empty;
    [Required(ErrorMessage = "Ответ не должен быть пустым.")]
    public string Answer { get; set; } = string.Empty;
}
