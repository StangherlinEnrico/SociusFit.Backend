using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Users;

public class GetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByIdUseCase> _logger;

    public GetUserByIdUseCase(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUserByIdUseCase> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UserDto>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return Result<UserDto>.Failure("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user: {UserId}", userId);
            return Result<UserDto>.Failure($"Failed to get user: {ex.Message}");
        }
    }
}