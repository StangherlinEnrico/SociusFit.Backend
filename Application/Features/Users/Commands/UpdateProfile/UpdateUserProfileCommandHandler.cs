using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.UpdateProfile;

/// <summary>
/// Handler for UpdateUserProfileCommand
/// </summary>
public class UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<UserDto>> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserDto>.FailureResult("User not found");
        }

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        return Result<UserDto>.SuccessResult(userDto, "Profile updated successfully");
    }
}
