using Application.Common.Models;
using Application.DTOs.AuditLogs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.AuditLogs.Queries.GetByUser;

public record GetAuditLogsByUserQuery : IRequest<Result<List<AuditLogDto>>>
{
    public int UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetAuditLogsByUserQueryHandler : IRequestHandler<GetAuditLogsByUserQuery, Result<List<AuditLogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAuditLogsByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<AuditLogDto>>> Handle(
        GetAuditLogsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _unitOfWork.AuditLogs.GetByUserIdAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var logDtos = _mapper.Map<List<AuditLogDto>>(logs.ToList());
        return Result<List<AuditLogDto>>.SuccessResult(logDtos);
    }
}