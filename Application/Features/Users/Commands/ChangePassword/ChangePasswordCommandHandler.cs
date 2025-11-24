using Application.Common.Models;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.ChangePassword;

/// <summary>
/// Handler for ChangePasswordCommand
/// </summary>
public class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : IRequestHandler<ChangePasswordCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<string>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<string>.FailureResult("User not found");
        }

        // Check if user has a password (OAuth-only users cannot change password)
        if (!user.HasPassword())
        {
            return Result<string>.FailureResult(
                "Cannot change password for accounts using social login. Please use your social provider to manage your account.");
        }

        // Verify current password
        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash!))
        {
            return Result<string>.FailureResult("Current password is incorrect");
        }

        // Check if new password is different from current
        if (_passwordHasher.VerifyPassword(request.NewPassword, user.PasswordHash!))
        {
            return Result<string>.FailureResult("New password must be different from current password");
        }

        // Hash and set new password
        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.SetPassword(newPasswordHash);

        // Revoke all other refresh tokens (keep current session or revoke all)
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(
            user.Id,
            request.IpAddress ?? "password-change",
            cancellationToken);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.SuccessResult(
            "Password changed successfully",
            "Your password has been changed. Please log in again with your new password.");
    }
}