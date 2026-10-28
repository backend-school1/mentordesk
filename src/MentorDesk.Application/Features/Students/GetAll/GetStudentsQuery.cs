using MediatR;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.GetAll;

public sealed record GetStudentsQuery()
    : IRequest<List<Student>>;
