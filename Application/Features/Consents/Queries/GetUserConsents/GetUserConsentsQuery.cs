using Application.Common.Models;
using Application.DTOs.Consents;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Consents.Queries.GetUserConsents;

public record GetUserConsentsQuery : IRequest<Result<List<UserConsentDto>>>
{
    public int UserId { get; init; }
}

public class GetUserConsentsQueryHandler : IRequestHandler<GetUserConsentsQuery, Result<List<UserConsentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserConsentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<UserConsentDto>>> Handle(
        GetUserConsentsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<List<UserConsentDto>>.FailureResult("User not found");
        }

        var consents = await _unitOfWork.UserConsents.GetByUserIdAsync(request.UserId, cancellationToken);
        var consentDtos = _mapper.Map<List<UserConsentDto>>(consents.ToList());
        return Result<List<UserConsentDto>>.SuccessResult(consentDtos);
    }
}