using System;

namespace MentorDesk.Domain.Enums;

/// <summary>
/// Thouse statuses are used to determine whether a student is active or archived. 
/// Active students are those who are currently enrolled in the program, 
/// while archived students are those who have completed the program or have been removed from it.
/// </summary

public enum StudentStatus
{
    Archived = 0,
    Active = 1
}
