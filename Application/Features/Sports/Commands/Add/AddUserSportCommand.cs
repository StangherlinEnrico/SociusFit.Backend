using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.UserSports.Commands.Add;

/// <summary>
/// Command to add sport to user
/// </summary>
public record AddUserSportCommand : IRequest<Result<UserSportDto>>
{
    public int UserId { get; init; }
    public int SportId { get; init; }
    public int LevelId { get; init; }
}

public class AddUserSportCommandHandler : IRequestHandler<AddUserSportCommand, Result<UserSportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddUserSportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserSportDto>> Handle(
        AddUserSportCommand request,
        CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserSportDto>.FailureResult("User not found");
        }

        // Check if sport exists
        var sport = await _unitOfWork.Sports.GetByIdAsync(request.SportId, cancellationToken);
        if (sport == null)
        {
            return Result<UserSportDto>.FailureResult("Sport not found");
        }

        // Check if level exists
        var level = await _unitOfWork.Levels.GetByIdAsync(request.LevelId, cancellationToken);
        if (level == null)
        {
            return Result<UserSportDto>.FailureResult("Level not found");
        }

        // Check if user already has this sport
        if (await _unitOfWork.UserSports.UserHasSportAsync(request.UserId, request.SportId, cancellationToken))
        {
            return Result<UserSportDto>.FailureResult("User already has this sport");
        }

        var userSport = new UserSport(request.UserId, request.SportId, request.LevelId);
        await _unitOfWork.UserSports.AddAsync(userSport, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load the full entity with navigation properties
        var savedUserSport = await _unitOfWork.UserSports.GetByUserAndSportAsync(
            request.UserId,
            request.SportId,
            cancellationToken);

        var userSportDto = _mapper.Map<UserSportDto>(savedUserSport);
        return Result<UserSportDto>.SuccessResult(userSportDto, "Sport added successfully");
    }
}
