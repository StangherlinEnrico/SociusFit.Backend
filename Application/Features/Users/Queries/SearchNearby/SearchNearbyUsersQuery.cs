using Application.Common.Models;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Queries.SearchNearby;

/// <summary>
/// Query to search users nearby
/// </summary>
public record SearchNearbyUsersQuery : IRequest<Result<List<UserSearchDto>>>
{
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public int MaxDistanceKm { get; init; }
    public int? SportId { get; init; }
    public int? LevelId { get; init; }
}

/// <summary>
/// Handler for SearchNearbyUsersQuery
/// </summary>
public class SearchNearbyUsersQueryHandler : IRequestHandler<SearchNearbyUsersQuery, Result<List<UserSearchDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationService _locationService;
    private readonly IMapper _mapper;

    public SearchNearbyUsersQueryHandler(
        IUnitOfWork unitOfWork,
        ILocationService locationService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _locationService = locationService;
        _mapper = mapper;
    }

    public async Task<Result<List<UserSearchDto>>> Handle(
        SearchNearbyUsersQuery request,
        CancellationToken cancellationToken)
    {
        // Get nearby users
        var nearbyUsers = await _unitOfWork.Users.GetUsersWithinDistanceAsync(
            request.Latitude,
            request.Longitude,
            request.MaxDistanceKm,
            cancellationToken);

        var usersList = nearbyUsers.ToList();

        // Filter by sport if specified
        if (request.SportId.HasValue)
        {
            var filteredUsers = new List<Domain.Entities.User>();
            foreach (var user in usersList)
            {
                var hasSport = await _unitOfWork.UserSports.UserHasSportAsync(
                    user.Id,
                    request.SportId.Value,
                    cancellationToken);

                if (hasSport)
                {
                    filteredUsers.Add(user);
                }
            }
            usersList = filteredUsers;
        }

        // Load sports for each user
        var results = new List<UserSearchDto>();
        foreach (var user in usersList)
        {
            var userSports = await _unitOfWork.UserSports.GetByUserIdAsync(user.Id, cancellationToken);

            var userDto = _mapper.Map<UserSearchDto>(user);
            userDto.DistanceKm = _locationService.CalculateDistance(
                request.Latitude,
                request.Longitude,
                user.Latitude ?? 0,
                user.Longitude ?? 0);

            userDto.Sports = _mapper.Map<List<UserSportDto>>(userSports);

            // Filter by level if specified
            if (request.LevelId.HasValue)
            {
                if (userDto.Sports.Any(s => s.SportId == request.SportId && s.LevelId == request.LevelId))
                {
                    results.Add(userDto);
                }
            }
            else
            {
                results.Add(userDto);
            }
        }

        // Sort by distance
        results = results.OrderBy(u => u.DistanceKm).ToList();

        return Result<List<UserSearchDto>>.SuccessResult(results);
    }
}