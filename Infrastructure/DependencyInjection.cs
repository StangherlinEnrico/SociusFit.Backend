using Domain.Common;
using Domain.Events;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Authentication;
using Infrastructure.Caching;
using Infrastructure.Events;
using Infrastructure.Events.Handlers;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddAuthentication(configuration);
        services.AddStorage(configuration);
        services.AddCaching(configuration);
        services.AddEventHandlers();

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<SociusFitDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ISportRepository, SportRepository>();

        // Token Management
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();

        return services;
    }

    private static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }

    private static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AzureBlobStorageSettings>(
            configuration.GetSection("AzureBlobStorageSettings"));

        services.AddScoped<IPhotoStorageRepository, AzureBlobPhotoStorageRepository>();

        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheType = configuration.GetValue<string>("CacheSettings:Type");

        if (cacheType?.ToLower() == "redis")
        {
            var redisConnection = configuration.GetValue<string>("CacheSettings:RedisConnection");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
            });

            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, InMemoryCacheService>();
        }

        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddScoped<IDomainEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<ProfileCreatedEvent>, ProfileCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<ProfileCompletedEvent>, ProfileCompletedEventHandler>();

        services.AddHttpClient<IGeocodingService, NominatimGeocodingService>();

        return services;
    }
}