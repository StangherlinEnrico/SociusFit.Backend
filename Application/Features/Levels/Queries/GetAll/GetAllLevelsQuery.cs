using Application.Common.Models;
using Application.DTOs.Sports;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Levels.Queries.GetAll;

/// <summary>
/// Query to get all levels
/// </summary>
public record GetAllLevelsQuery : IRequest<Result<List<LevelDto>>>;

public class GetAllLevelsQueryHandler : IRequestHandler<GetAllLevelsQuery, Result<List<LevelDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLevelsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<LevelDto>>> Handle(
        GetAllLevelsQuery request,
        CancellationToken cancellationToken)
    {
        var levels = await _unitOfWork.Levels.GetAllAsync(cancellationToken);
        var levelDtos = _mapper.Map<List<LevelDto>>(levels.ToList());
        return Result<List<LevelDto>>.SuccessResult(levelDtos);
    }
}
