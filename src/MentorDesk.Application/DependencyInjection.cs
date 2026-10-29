using FluentValidation;
using MediatR;
using MentorDesk.Application.Behaviors;
using MentorDesk.Application.Features.Decks.Import;
using Microsoft.Extensions.DependencyInjection;

namespace MentorDesk.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
