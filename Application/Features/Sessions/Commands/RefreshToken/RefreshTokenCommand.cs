using Application.Common.Models;
using Application.DTOs.Sessions;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Sessions.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<Result<SessionDto>>
{
    public string Token { get; init; } = string.Empty;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<SessionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenGenerator tokenGenerator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<SessionDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var oldSession = await _unitOfWork.Sessions.GetByTokenAsync(request.Token, cancellationToken);
        if (oldSession == null)
        {
            return Result<SessionDto>.FailureResult("Session not found");
        }

        _unitOfWork.Sessions.Remove(oldSession);

        var newToken = _tokenGenerator.GenerateToken();
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var newSession = new Session(oldSession.UserId, newToken, expiresAt);

        await _unitOfWork.Sessions.AddAsync(newSession, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sessionDto = _mapper.Map<SessionDto>(newSession);
        return Result<SessionDto>.SuccessResult(sessionDto, "Token refreshed successfully");
    }
}