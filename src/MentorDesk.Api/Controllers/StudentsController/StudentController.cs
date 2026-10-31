using MediatR;
using MentorDesk.Application.Features.Students.ChangeStatus;
using MentorDesk.Application.Features.Students.Create;
using MentorDesk.Application.Features.Students.GetAll;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;
using Microsoft.AspNetCore.Mvc;


namespace MentorDesk.Api.Controllers.StudentsController;

[ApiController]
[Route("api/[controller]")]

public sealed class StudentController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetStudentsQuery(), ct);

        return Ok(result.Value!.Select(ToResponse));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(StudentsDTO.CreateStudentRequest request, CancellationToken ct = default)
    {
        var command = new CreateStudentCommand(
            request.telegramUserName);

        var result = await sender.Send(command, ct);

        if (!result.IsSuccess)
            return Conflict(result.ErrorMessage);

        return StatusCode(
            StatusCodes.Status201Created,
            ToResponse(result.Value!));
    }

    
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        StudentsDTO.ChangeStudentStatusRequest request,
        CancellationToken ct)
    {
        var command = new ChangeStudentStatusCommand(
            id, request.Status);

        var result = await sender.Send(command, ct);

        return Ok(result);
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
