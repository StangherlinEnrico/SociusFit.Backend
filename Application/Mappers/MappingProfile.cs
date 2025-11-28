using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Profile, ProfileDto>()
            .ForMember(dest => dest.IsComplete, opt => opt.MapFrom(src => src.IsComplete()));

        CreateMap<Sport, SportDto>();
    }
}