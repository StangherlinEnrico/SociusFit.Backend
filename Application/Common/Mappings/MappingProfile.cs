using Application.DTOs.AuditLogs;
using Application.DTOs.Consents;
using Application.DTOs.Sessions;
using Application.DTOs.Sports;
using Application.DTOs.Users;
using AutoMapper;
using Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Common.Mappings;

/// <summary>
/// AutoMapper profiles for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified()));

        CreateMap<User, UserWithSportsDto>()
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified()))
            .ForMember(dest => dest.Sports, opt => opt.MapFrom(src => src.UserSports));

        CreateMap<User, UserSearchDto>()
            .ForMember(dest => dest.Sports, opt => opt.MapFrom(src => src.UserSports))
            .ForMember(dest => dest.DistanceKm, opt => opt.Ignore());

        // UserSport mappings
        CreateMap<UserSport, UserSportDto>()
            .ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.Name))
            .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.Level.Name));

        // Sport mappings
        CreateMap<Sport, SportDto>();
        CreateMap<CreateSportDto, Sport>()
            .ConstructUsing(dto => new Sport(dto.Name));

        // Level mappings
        CreateMap<Level, LevelDto>();
        CreateMap<CreateLevelDto, Level>()
            .ConstructUsing(dto => new Level(dto.Name));

        // Session mappings
        CreateMap<Session, SessionDto>();

        // UserConsent mappings
        CreateMap<UserConsent, UserConsentDto>();

        // AuditLog mappings
        CreateMap<AuditLog, AuditLogDto>();
    }
}