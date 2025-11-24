using Application.Common.Models;
using Domain.Constants;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.ForgotPassword;

/// <summary>
/// Handler for ForgotPasswordCommand
/// </summary>
public class ForgotPasswordCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenGenerator tokenGenerator,
    IEmailService emailService) : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result<string>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Always return success to prevent email enumeration attacks
        const string successMessage = "If an account with that email exists, a password reset link has been sent.";

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            // Don't reveal if email exists
            return Result<string>.SuccessResult(successMessage);
        }

        // OAuth-only users cannot reset password
        if (user.HasOAuthProvider() && !user.HasPassword())
        {
            // Don't reveal account type for security
            return Result<string>.SuccessResult(successMessage);
        }

        // Generate password reset token
        var resetToken = _tokenGenerator.GenerateToken();
        var tokenExpiresAt = DateTime.UtcNow.AddHours(AuthConstants.PasswordResetTokenExpirationHours);
        user.SetPasswordResetToken(resetToken, tokenExpiresAt);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send password reset email
        try
        {
            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                resetToken,
                cancellationToken);
        }
        catch
        {
            // Log error but don't reveal to user
            // In production, log this error
        }

        return Result<string>.SuccessResult(successMessage);
    }
}