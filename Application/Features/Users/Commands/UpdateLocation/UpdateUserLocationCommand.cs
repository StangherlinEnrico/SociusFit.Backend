using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.UpdateLocation;

/// <summary>
/// Command to update user location
/// </summary>
public record UpdateUserLocationCommand : IRequest<Result<UserDto>>
{
    public int UserId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public int MaxDistanceKm { get; init; }
}

/// <summary>
/// Handler for UpdateUserLocationCommand
/// </summary>
public class UpdateUserLocationCommandHandler : IRequestHandler<UpdateUserLocationCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserLocationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(
        UpdateUserLocationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserDto>.FailureResult("User not found");
        }

        user.SetLocation(request.Latitude, request.Longitude, request.MaxDistanceKm);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.SuccessResult(userDto, "Location updated successfully");
    }
}