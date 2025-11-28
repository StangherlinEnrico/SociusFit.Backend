using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Infrastructure.Authentication;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users;

public class LoginUserUseCase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginUserUseCase> _logger;

    public LoginUserUseCase(
        IAuthenticationService authenticationService,
        IMapper mapper,
        ILogger<LoginUserUseCase> logger)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (user, token) = await _authenticationService.LoginAsync(
                request.Email,
                request.Password,
                cancellationToken
            );

            var userDto = _mapper.Map<UserDto>(user);
            var response = new AuthResponseDto
            {
                User = userDto,
                Token = token
            };

            _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

            return Result<AuthResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Login failed for email: {Email}", request.Email);
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }
    }
}