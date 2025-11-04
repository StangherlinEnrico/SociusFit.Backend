using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Queries.GetUserWithSports;

/// <summary>
/// Query to get user with sports
/// </summary>
public record GetUserWithSportsQuery : IRequest<Result<UserWithSportsDto>>
{
    public int UserId { get; init; }
}

/// <summary>
/// Handler for GetUserWithSportsQuery
/// </summary>
public class GetUserWithSportsQueryHandler : IRequestHandler<GetUserWithSportsQuery, Result<UserWithSportsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserWithSportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserWithSportsDto>> Handle(
        GetUserWithSportsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdWithSportsAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserWithSportsDto>.FailureResult("User not found");
        }

        var userDto = _mapper.Map<UserWithSportsDto>(user);
        return Result<UserWithSportsDto>.SuccessResult(userDto);
    }
}

