using Application.DTOs;
using Application.Requests;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Infrastructure.Authentication;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterUserUseCase> _logger;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IAuthenticationService authenticationService,
        IDomainEventDispatcher eventDispatcher,
        IMapper mapper,
        ILogger<RegisterUserUseCase> logger)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
        _eventDispatcher = eventDispatcher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AuthResponseDto>> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                return Result<AuthResponseDto>.Failure("User with this email already exists");
            }

            var user = new User(request.FirstName, request.LastName, request.Email);

            var (registeredUser, token) = await _authenticationService.RegisterAsync(
                user,
                request.Password,
                cancellationToken
            );

            await _eventDispatcher.DispatchAsync(
                new UserCreatedEvent(registeredUser.Id, registeredUser.Email, registeredUser.FirstName, registeredUser.LastName),
                cancellationToken
            );

            var userDto = _mapper.Map<UserDto>(registeredUser);
            var response = new AuthResponseDto
            {
                User = userDto,
                Token = token
            };

            _logger.LogInformation("User registered successfully: {UserId}", registeredUser.Id);

            return Result<AuthResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user with email: {Email}", request.Email);
            return Result<AuthResponseDto>.Failure($"Registration failed: {ex.Message}");
        }
    }
}