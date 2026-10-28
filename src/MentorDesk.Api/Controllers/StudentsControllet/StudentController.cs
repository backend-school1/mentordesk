using MediatR;
using MentorDesk.Api.Students;
using MentorDesk.Application.Features.Students.ChangeStatus;
using MentorDesk.Application.Features.Students.Create;
using MentorDesk.Application.Features.Students.GetAll;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;
using Microsoft.AspNetCore.Mvc;


namespace MentorDesk.Api.Students;

[ApiController]
[Route("students")]

public sealed class StudentController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken ct = default)
    {
        var query = new GetStudentsQuery();

        var students = await sender.Send(query, ct);

        return Ok(students.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(StudentsDTO.CreateStudentRequest request, CancellationToken ct = default)
    {
        try
        {
            var command = new CreateStudentCommand(
                request.telegramUserName);

            var student = await sender.Send(command, ct);

            if (student is null)
                return Conflict("Student already exists");

            return StatusCode(
                StatusCodes.Status201Created,
                ToResponse(student));
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }



    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStudentStatus(
        Guid id,
        StudentsDTO.ChangeStudentStatusRequest request,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<StudentStatus>(
                request.status, true, out var status)
            || !Enum.IsDefined(status))
        {
            return BadRequest("Invalid status");
        }

        var command = new ChangeStudentStatusCommand(
            id,
            status);

        var student = await sender.Send(command, ct);

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
