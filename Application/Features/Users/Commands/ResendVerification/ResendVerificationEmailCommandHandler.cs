using Application.Common.Models;
using Domain.Constants;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands.ResendVerification;

/// <summary>
/// Handler for ResendVerificationEmailCommand
/// </summary>
public class ResendVerificationEmailCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenGenerator tokenGenerator,
    IEmailService emailService) : IRequestHandler<ResendVerificationEmailCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IEmailService _emailService = emailService;

    public async Task<Result<string>> Handle(
        ResendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            // Don't reveal if email exists for security reasons
            return Result<string>.SuccessResult(
                "If the email exists, a verification link has been sent.",
                "If your email is registered, you will receive a verification link shortly.");
        }

        // Check if already verified
        if (user.IsEmailVerified())
        {
            return Result<string>.FailureResult("Email is already verified");
        }

        // Generate new verification token
        var verificationToken = _tokenGenerator.GenerateToken();
        var tokenExpiresAt = DateTime.UtcNow.AddHours(AuthConstants.EmailVerificationTokenExpirationHours);
        user.SetEmailVerificationToken(verificationToken, tokenExpiresAt);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send verification email
        try
        {
            await _emailService.SendVerificationEmailAsync(
                user.Email,
                user.FirstName,
                verificationToken,
                cancellationToken);
        }
        catch
        {
            return Result<string>.FailureResult("Failed to send verification email. Please try again later.");
        }

        return Result<string>.SuccessResult(
            "Verification email sent",
            "A new verification link has been sent to your email address.");
    }
}