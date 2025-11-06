using Application.Common.Models;
using Application.DTOs.AuditLogs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.AuditLogs.Queries.GetByTable;

public record GetAuditLogsByTableQuery : IRequest<Result<List<AuditLogDto>>>
{
    public string TableName { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetAuditLogsByTableQueryHandler : IRequestHandler<GetAuditLogsByTableQuery, Result<List<AuditLogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAuditLogsByTableQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<AuditLogDto>>> Handle(
        GetAuditLogsByTableQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _unitOfWork.AuditLogs.GetByTableNameAsync(
            request.TableName,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var logDtos = _mapper.Map<List<AuditLogDto>>(logs.ToList());
        return Result<List<AuditLogDto>>.SuccessResult(logDtos);
    }
}