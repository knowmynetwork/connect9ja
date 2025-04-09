using AutoMapper;
using Datify.API.Data;
using Datify.Shared.Models;

namespace Datify.API.Configuration;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        //user
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
        CreateMap<ApplicationUser, RegisterModelDto>().ReverseMap();
        CreateMap<Message, MessageDto>().ReverseMap();
        
        //property
        CreateMap<DatifyProfile, DatifyProfileDto>().ReverseMap()
            .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore())
            .ForMember(dest => dest.CoverPhoto, opt => opt.Ignore()); // File handled manually
    }

}
