using Application.Common.Models;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Command to soft delete a user
/// </summary>
public record DeleteUserCommand : IRequest<Result>
{
    public int UserId { get; init; }
}

/// <summary>
/// Handler for DeleteUserCommand
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.FailureResult("User not found");
        }

        user.SoftDelete();

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("User deleted successfully");
    }
}