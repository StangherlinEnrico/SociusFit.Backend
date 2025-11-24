using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.UpdateLocation;

/// <summary>
/// Handler for UpdateUserLocationCommand
/// </summary>
public class UpdateUserLocationCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<UpdateUserLocationCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<UserDto>> Handle(
        UpdateUserLocationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.FailureResult("User not found");
        }

        // Update location settings using domain method
        user.SetLocation(request.Location, request.MaxDistance);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.SuccessResult(userDto, "Location settings updated successfully");
    }
}