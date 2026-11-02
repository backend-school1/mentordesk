using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.Create;

public sealed record CreateStudentCommand(string TelegramUserName) : IRequest<Result<Student>>;
