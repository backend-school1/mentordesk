using MediatR;

namespace MentorDesk.Application.Features.Students.Access;

public sealed record CheckStudentAccessQuery(long TelegramId, string UserName) : IRequest<StudentAccessResult>;

