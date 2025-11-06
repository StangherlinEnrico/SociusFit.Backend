using Application.Common.Models;
using Application.DTOs.AuditLogs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.AuditLogs.Queries.GetByRecord;

public record GetAuditLogsByRecordQuery : IRequest<Result<List<AuditLogDto>>>
{
    public string TableName { get; init; } = string.Empty;
    public int RecordId { get; init; }
}

public class GetAuditLogsByRecordQueryHandler : IRequestHandler<GetAuditLogsByRecordQuery, Result<List<AuditLogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAuditLogsByRecordQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<AuditLogDto>>> Handle(
        GetAuditLogsByRecordQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _unitOfWork.AuditLogs.GetByRecordAsync(
            request.TableName,
            request.RecordId,
            cancellationToken);

        var logDtos = _mapper.Map<List<AuditLogDto>>(logs.ToList());
        return Result<List<AuditLogDto>>.SuccessResult(logDtos);
    }
}