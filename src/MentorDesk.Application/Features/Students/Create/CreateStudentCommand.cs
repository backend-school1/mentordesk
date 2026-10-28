using MediatR;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.Create;

public sealed record CreateStudentCommand(string TelegramUsername) : IRequest<Student>;
