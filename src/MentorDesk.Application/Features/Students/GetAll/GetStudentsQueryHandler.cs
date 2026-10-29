using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Common;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.GetAll;

public sealed class GetStudentsQueryHandler(
    IStudentRepository repository)
    : IRequestHandler<GetStudentsQuery, Result<List<Student>>>
{
    public async Task<Result<List<Student>>> Handle(
        GetStudentsQuery request,
        CancellationToken cancellationToken)
    {
        var students = await repository.GetAllAsync(
            cancellationToken);

        return Result<List<Student>>.Success(students);
    }
}
