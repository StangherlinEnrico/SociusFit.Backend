using Application.Common.Models;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Sessions.Commands.Logout;

public record LogoutCommand : IRequest<Result>
{
    public string Token { get; init; } = string.Empty;
}

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.Sessions.GetByTokenAsync(request.Token, cancellationToken);
        if (session == null)
        {
            return Result.FailureResult("Session not found");
        }

        _unitOfWork.Sessions.Remove(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Logged out successfully");
    }
}