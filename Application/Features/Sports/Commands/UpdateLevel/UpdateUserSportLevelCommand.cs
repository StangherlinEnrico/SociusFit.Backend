using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.UserSports.Commands.UpdateLevel;

/// <summary>
/// Command to update user sport level
/// </summary>
public record UpdateUserSportLevelCommand : IRequest<Result<UserSportDto>>
{
    public int UserId { get; init; }
    public int SportId { get; init; }
    public int NewLevelId { get; init; }
}

public class UpdateUserSportLevelCommandHandler : IRequestHandler<UpdateUserSportLevelCommand, Result<UserSportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserSportLevelCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserSportDto>> Handle(
        UpdateUserSportLevelCommand request,
        CancellationToken cancellationToken)
    {
        var userSport = await _unitOfWork.UserSports.GetByUserAndSportAsync(
            request.UserId,
            request.SportId,
            cancellationToken);

        if (userSport == null)
        {
            return Result<UserSportDto>.FailureResult("User sport not found");
        }

        // Check if level exists
        var level = await _unitOfWork.Levels.GetByIdAsync(request.NewLevelId, cancellationToken);
        if (level == null)
        {
            return Result<UserSportDto>.FailureResult("Level not found");
        }

        userSport.UpdateLevel(request.NewLevelId);
        _unitOfWork.UserSports.Update(userSport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userSportDto = _mapper.Map<UserSportDto>(userSport);
        return Result<UserSportDto>.SuccessResult(userSportDto, "Level updated successfully");
    }
}
