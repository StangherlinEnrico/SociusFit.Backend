using Application.UseCases.Discovery;
using Application.UseCases.Matches;
using Application.UseCases.Profiles;
using Application.UseCases.Users;
using Application.Validators;
using Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper();
        services.AddValidators();
        services.AddDomainValidators();
        services.AddUseCases();

        return services;
    }

    private static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(DependencyInjection).Assembly);
        });
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }

    private static IServiceCollection AddDomainValidators(this IServiceCollection services)
    {
        services.AddSingleton<UserValidator>();
        services.AddSingleton<ProfileValidator>();
        services.AddSingleton<SportValidator>();
        services.AddSingleton<PhotoValidator>();
        services.AddScoped<DeleteAccountRequestValidator>();

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<LoginUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<LogoutUserUseCase>();
        services.AddScoped<DeleteAccountUseCase>();

        services.AddScoped<CreateProfileUseCase>();
        services.AddScoped<GetProfileByUserIdUseCase>();
        services.AddScoped<UpdateProfileUseCase>();
        services.AddScoped<UploadProfilePhotoUseCase>();

        services.AddScoped<GetDiscoveryCardsUseCase>();
        services.AddScoped<SwipeLikeUseCase>();
        services.AddScoped<GetMatchesUseCase>();

        return services;
    }
}