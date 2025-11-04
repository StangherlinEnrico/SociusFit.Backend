using Application.Common.Models;
using Application.DTOs.Sports;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Sports.Commands.Create;

/// <summary>
/// Command to create a new sport
/// </summary>
public record CreateSportCommand : IRequest<Result<SportDto>>
{
    public string Name { get; init; } = string.Empty;
}

public class CreateSportCommandHandler : IRequestHandler<CreateSportCommand, Result<SportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SportDto>> Handle(CreateSportCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Sports.NameExistsAsync(request.Name, null, cancellationToken))
        {
            return Result<SportDto>.FailureResult("Sport already exists");
        }

        var sport = new Sport(request.Name);
        await _unitOfWork.Sports.AddAsync(sport, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var sportDto = _mapper.Map<SportDto>(sport);
        return Result<SportDto>.SuccessResult(sportDto, "Sport created successfully");
    }
}
