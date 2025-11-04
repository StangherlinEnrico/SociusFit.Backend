using Application.Common.Models;
using Application.DTOs.Sports;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Sports.Queries.GetPopular;

/// <summary>
/// Query to get popular sports
/// </summary>
public record GetPopularSportsQuery : IRequest<Result<List<SportDto>>>
{
    public int Count { get; init; } = 10;
}

public class GetPopularSportsQueryHandler : IRequestHandler<GetPopularSportsQuery, Result<List<SportDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPopularSportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<SportDto>>> Handle(
        GetPopularSportsQuery request,
        CancellationToken cancellationToken)
    {
        var sports = await _unitOfWork.Sports.GetPopularSportsAsync(request.Count, cancellationToken);
        var sportDtos = _mapper.Map<List<SportDto>>(sports.ToList());
        return Result<List<SportDto>>.SuccessResult(sportDtos);
    }
}
