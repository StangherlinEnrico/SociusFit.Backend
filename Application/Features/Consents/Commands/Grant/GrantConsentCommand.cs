using Application.Common.Models;
using Application.DTOs.Consents;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Consents.Commands.Grant;

public record GrantConsentCommand : IRequest<Result<UserConsentDto>>
{
    public int UserId { get; init; }
    public string ConsentType { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}

public class GrantConsentCommandHandler : IRequestHandler<GrantConsentCommand, Result<UserConsentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GrantConsentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserConsentDto>> Handle(
        GrantConsentCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserConsentDto>.FailureResult("User not found");
        }

        var existingConsent = await _unitOfWork.UserConsents.GetByUserAndTypeAsync(
            request.UserId,
            request.ConsentType,
            cancellationToken);

        if (existingConsent != null)
        {
            existingConsent.Grant(request.IpAddress);
            _unitOfWork.UserConsents.Update(existingConsent);
        }
        else
        {
            existingConsent = new UserConsent(request.UserId, request.ConsentType, request.IpAddress);
            existingConsent.Grant(request.IpAddress);
            await _unitOfWork.UserConsents.AddAsync(existingConsent, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var consentDto = _mapper.Map<UserConsentDto>(existingConsent);
        return Result<UserConsentDto>.SuccessResult(consentDto, "Consent granted successfully");
    }
}