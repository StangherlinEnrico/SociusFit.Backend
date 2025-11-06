using Application.Common.Models;
using Application.DTOs.Sessions;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Sessions.Queries.GetActiveSessions;

public record GetActiveSessionsQuery : IRequest<Result<List<SessionDto>>>
{
    public int UserId { get; init; }
}

public class GetActiveSessionsQueryHandler : IRequestHandler<GetActiveSessionsQuery, Result<List<SessionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActiveSessionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<SessionDto>>> Handle(
        GetActiveSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<List<SessionDto>>.FailureResult("User not found");
        }

        var sessions = await _unitOfWork.Sessions.GetActiveSessionsByUserIdAsync(
            request.UserId,
            cancellationToken);

        var sessionDtos = _mapper.Map<List<SessionDto>>(sessions.ToList());
        return Result<List<SessionDto>>.SuccessResult(sessionDtos);
    }
}