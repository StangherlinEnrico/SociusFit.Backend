using Application.Common.Models;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.UserSports.Commands.Remove;

/// <summary>
/// Command to remove sport from user
/// </summary>
public record RemoveUserSportCommand : IRequest<Result>
{
    public int UserId { get; init; }
    public int SportId { get; init; }
}

public class RemoveUserSportCommandHandler : IRequestHandler<RemoveUserSportCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserSportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveUserSportCommand request, CancellationToken cancellationToken)
    {
        var userSport = await _unitOfWork.UserSports.GetByUserAndSportAsync(
            request.UserId,
            request.SportId,
            cancellationToken);

        if (userSport == null)
        {
            return Result.FailureResult("User sport not found");
        }

        _unitOfWork.UserSports.Remove(userSport);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Sport removed successfully");
    }
}