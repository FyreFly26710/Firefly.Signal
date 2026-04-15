using System.Reflection;
using Firefly.Signal.SharedKernel.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Signal.SharedKernel.Extensions;

public static class MediatorExtensions
{
    public static IServiceCollection AddFireflyMediator(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            config.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        return services;
    }
}
