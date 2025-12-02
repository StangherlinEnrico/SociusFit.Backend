using Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace API.Middleware;

/// <summary>
/// Middleware that checks if the JWT token has been revoked (blacklisted)
/// Must be placed AFTER authentication middleware
/// </summary>
public class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenRevocationMiddleware> _logger;

    public TokenRevocationMiddleware(
        RequestDelegate next,
        ILogger<TokenRevocationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        // Only check authenticated requests
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var isRevoked = await tokenService.IsTokenRevokedAsync(token, context.RequestAborted);

                if (isRevoked)
                {
                    _logger.LogWarning("Attempt to use revoked token by user {UserId}",
                        context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = "Token has been revoked",
                        code = "TOKEN_REVOKED"
                    }));

                    return;
                }
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method for adding TokenRevocationMiddleware to pipeline
/// </summary>
public static class TokenRevocationMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenRevocationValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenRevocationMiddleware>();
    }
}