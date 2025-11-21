using Domain.Interfaces;
using Domain.Interfaces.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Dependency Injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services to the service collection
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout(30);
            });

            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }

            // Enable detailed errors in development
            if (configuration.GetValue<bool>("EnableDetailedErrors"))
            {
                options.EnableDetailedErrors();
            }
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Domain Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuditService, AuditService>();

        // OAuth Service with HttpClient
        services.AddHttpClient<IOAuthService, OAuthService>();

        return services;
    }

    /// <summary>
    /// Adds Infrastructure services with a custom database provider
    /// </summary>
    public static IServiceCollection AddInfrastructureWithProvider(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        // Database Context with custom provider
        services.AddDbContext<AppDbContext>(optionsAction);

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Domain Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuditService, AuditService>();

        // OAuth Service with HttpClient
        services.AddHttpClient<IOAuthService, OAuthService>();

        return services;
    }
}