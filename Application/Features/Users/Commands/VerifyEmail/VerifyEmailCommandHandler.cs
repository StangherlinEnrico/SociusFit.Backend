using Application.Common.Models;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.VerifyEmail;

/// <summary>
/// Handler for VerifyEmailCommand
/// </summary>
public class VerifyEmailCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailService emailService) : IRequestHandler<VerifyEmailCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result<string>> Handle(
        VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Result<string>.FailureResult("Invalid verification token");
        }

        // Find user by verification token
        var user = await _unitOfWork.Users.GetByEmailVerificationTokenAsync(
            request.Token,
            cancellationToken);

        if (user == null)
        {
            return Result<string>.FailureResult("Invalid or expired verification token");
        }

        // Check if already verified
        if (user.IsEmailVerified())
        {
            return Result<string>.SuccessResult(
                "Email already verified",
                "Your email has already been verified. You can log in now.");
        }

        // Validate token expiration
        if (!user.IsEmailVerificationTokenValid(request.Token))
        {
            return Result<string>.FailureResult("Verification token has expired. Please request a new one.");
        }

        // Verify email
        user.VerifyEmail();
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send welcome email
        try
        {
            await _emailService.SendWelcomeEmailAsync(
                user.Email,
                user.FirstName,
                cancellationToken);
        }
        catch
        {
            // Log error but don't fail verification
        }

        return Result<string>.SuccessResult(
            "Email verified successfully",
            "Your email has been verified successfully! You can now access all features.");
    }
}