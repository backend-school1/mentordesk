using System;
using MentorDesk.Domain.Enums;


namespace MentorDesk.Domain.Entities;

/// <summary>
/// Entity representing a student in the MentorDesk system.
/// </summary>
/// 

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid ();

    public string TelegramUserName { get; set; } = string.Empty;

    public long? TelegramId { get; set; } = null;

    public  StudentStatus Status { get; set; } = StudentStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
