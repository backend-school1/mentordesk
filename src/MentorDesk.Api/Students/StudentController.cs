using MentorDesk.Api.Students;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;


namespace MentorDesk.Api.Students;

[ApiController]
[Route("students")]

public sealed class StudentController(StudentService studentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var students = await studentService.GetAllAsync(ct);
        return Ok(students.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(StudentsDTO.CreateStudentRequest request, CancellationToken ct = default)
    {
        try
        {

            var student = await studentService.CreateAsync(request.telegramUserName, ct);

            if (student is null)
                return Conflict("Student already exists");

            return StatusCode(StatusCodes.Status201Created, ToResponse(student));
        }

        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStudentStatus(Guid id, StudentsDTO.ChangeStudentStatusRequest request,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<StudentStatus>(request.status, true, out var status) || !Enum.IsDefined(typeof(StudentStatus), status))
            return BadRequest("Invalid status");

        var student = await studentService.ChangeStatusAsync(id, status, ct);

        if (student is null)
            return NotFound("Student not found");

        return Ok(ToResponse(student));
    }


    private static StudentsDTO.StudentResponse ToResponse(Student s)
    {
        return new StudentsDTO.StudentResponse(
            s.Id,
            s.TelegramUserName,
            s.TelegramId,
            s.Status.ToString(),
            s.CreatedAt);
    }

}
