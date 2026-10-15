using System;
using MentorDesk.Domain.Enums;


namespace MentorDesk.Domain.Entities;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid ();

    public string TelegrammUserName { get; set; } = string.Empty;

    public long? TelegrammId { get; set; } = null;

    public  StudentStatus Status { get; set; } = StudentStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
