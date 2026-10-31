using MediatR;
using MentorDesk.Application.Common;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.GetAll;

public sealed record GetStudentsQuery()
    : IRequest<Result<List<Student>>>;
