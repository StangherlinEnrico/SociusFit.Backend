using Application.Common.Models;
using Application.DTOs.Sports;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Sports.Queries.GetAll;

/// <summary>
/// Query to get all sports
/// </summary>
public record GetAllSportsQuery : IRequest<Result<List<SportDto>>>;

public class GetAllSportsQueryHandler : IRequestHandler<GetAllSportsQuery, Result<List<SportDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<SportDto>>> Handle(
        GetAllSportsQuery request,
        CancellationToken cancellationToken)
    {
        var sports = await _unitOfWork.Sports.GetAllAsync(cancellationToken);
        var sportDtos = _mapper.Map<List<SportDto>>(sports.ToList());
        return Result<List<SportDto>>.SuccessResult(sportDtos);
    }
}
