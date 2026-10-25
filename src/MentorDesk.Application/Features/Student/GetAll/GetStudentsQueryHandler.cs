using MediatR;
using MentorDesk.Application.Students;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Students.GetAll;

public sealed class GetStudentsQueryHandler(
    IStudentRepository repository)
    : IRequestHandler<GetStudentsQuery, List<Student>>
{
    public async Task<List<Student>> Handle(
        GetStudentsQuery request,
        CancellationToken cancellationToken)
    {
        return await repository.GetAllAsync(cancellationToken);
    }
}
