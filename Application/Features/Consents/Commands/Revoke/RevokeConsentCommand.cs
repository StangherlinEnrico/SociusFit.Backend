using Application.Common.Models;
using Application.DTOs.Consents;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Consents.Commands.Revoke;

public record RevokeConsentCommand : IRequest<Result<UserConsentDto>>
{
    public int UserId { get; init; }
    public string ConsentType { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}

public class RevokeConsentCommandHandler : IRequestHandler<RevokeConsentCommand, Result<UserConsentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RevokeConsentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserConsentDto>> Handle(
        RevokeConsentCommand request,
        CancellationToken cancellationToken)
    {
        var consent = await _unitOfWork.UserConsents.GetByUserAndTypeAsync(
            request.UserId,
            request.ConsentType,
            cancellationToken);

        if (consent == null)
        {
            return Result<UserConsentDto>.FailureResult("Consent not found");
        }

        consent.Revoke(request.IpAddress);
        _unitOfWork.UserConsents.Update(consent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var consentDto = _mapper.Map<UserConsentDto>(consent);
        return Result<UserConsentDto>.SuccessResult(consentDto, "Consent revoked successfully");
    }
}