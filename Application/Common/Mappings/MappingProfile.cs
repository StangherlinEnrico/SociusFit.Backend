using Application.DTOs.AuditLogs;
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

        // AuditLog mappings
        CreateMap<AuditLog, AuditLogDto>();
    }
}