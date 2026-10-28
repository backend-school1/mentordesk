using MediatR;
using MentorDesk.Domain.Entities;
using MentorDesk.Domain.Enums;

namespace MentorDesk.Application.Features.Students.ChangeStatus;

public sealed record ChangeStudentStatusCommand(
    Guid StudentId,
    StudentStatus Status
) : IRequest<Student>;
