using System.Reflection;
using FluentValidation;
using MediatR;
using Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // AutoMapper - CORRETTO
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(assembly);
        });

        // FluentValidation - CORRETTO
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}