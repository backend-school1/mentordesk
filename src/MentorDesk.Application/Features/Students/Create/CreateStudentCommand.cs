using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.Create;

public sealed record CreateStudentCommand(string TelegramUsername) : IRequest<Result<Student>>;
