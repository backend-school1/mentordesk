using MentorDesk.Application.Features.Decks.Import;
using Microsoft.Extensions.DependencyInjection;

namespace MentorDesk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c
            => c.RegisterServicesFromAssemblyContaining<ImportDeckCommand>());

        return services;
    }
}
