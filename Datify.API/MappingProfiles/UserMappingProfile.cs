using AutoMapper;
using Datify.API.Data;
using Datify.Shared.Models;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, UserProfileDto>()
            .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.Interests))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));
        CreateMap<Interest, InterestDto>();
        CreateMap<Photo, PhotoDto>();
    }
}