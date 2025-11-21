using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Queries.GetUser;

/// <summary>
/// Query to get user by ID
/// </summary>
public record GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public int UserId { get; init; }
}

/// <summary>
/// Handler for GetUserByIdQuery
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserDto>.FailureResult("User not found");
        }

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.SuccessResult(userDto);
    }
}
