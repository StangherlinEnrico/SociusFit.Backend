using Application.DTOs;
using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Sports;

public class GetAllSportsUseCase
{
    private readonly ISportRepository _sportRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllSportsUseCase> _logger;

    public GetAllSportsUseCase(
        ISportRepository sportRepository,
        IMapper mapper,
        ILogger<GetAllSportsUseCase> logger)
    {
        _sportRepository = sportRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<SportDto>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var sports = await _sportRepository.GetAllAsync(cancellationToken);
            var sportDtos = _mapper.Map<List<SportDto>>(sports);

            _logger.LogInformation("Retrieved {Count} sports", sportDtos.Count);

            return Result<List<SportDto>>.Success(sportDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sports list");
            return Result<List<SportDto>>.Failure($"Failed to retrieve sports: {ex.Message}");
        }
    }
}