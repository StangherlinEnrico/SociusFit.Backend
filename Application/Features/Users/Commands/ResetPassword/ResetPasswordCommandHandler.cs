using Application.Common.Models;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.ResetPassword;

/// <summary>
/// Handler for ResetPasswordCommand
/// </summary>
public class ResetPasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<string>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Find user by reset token
        var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(
            request.Token,
            cancellationToken);

        if (user == null)
        {
            return Result<string>.FailureResult("Invalid or expired password reset token");
        }

        // Validate token
        if (!user.IsPasswordResetTokenValid(request.Token))
        {
            return Result<string>.FailureResult("Password reset token has expired. Please request a new one.");
        }

        // Hash and set new password (this also clears the reset token)
        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.SetPassword(passwordHash);

        // Revoke all existing refresh tokens for security
        await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(
            user.Id,
            "password-reset",
            cancellationToken);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.SuccessResult(
            "Password reset successful",
            "Your password has been reset successfully. You can now log in with your new password.");
    }
}