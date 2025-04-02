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
        
        //property
        CreateMap<Property, PropertyDto>().ReverseMap();
        CreateMap<CreatePropertyDto, Property>()
            .ForMember(dest => dest.PropertyItemDocuments, opt => opt.Ignore()) // Ignoring ICollection, as it needs explicit handling
            .ForMember(dest => dest.Features, opt => opt.Ignore())
            .ForMember(dest => dest.ProximityPlaces, opt => opt.Ignore())
            .ForMember(dest => dest.Ratings, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyRules, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyAllowedEvents, opt => opt.Ignore());
        CreateMap<PropertyProximityPlace, PropertyProximityPlaceDto>().ReverseMap();
        CreateMap<PropertyFeature, PropertyFeatureDto>().ReverseMap();
        CreateMap<PropertyDocument, PropertyDocumentDto>().ReverseMap();
        CreateMap<PropertyRatings, PropertyRatingDto>();
        CreateMap<CreatePropertyRatingDto, PropertyRatings>();
        CreateMap<BookingCreationDto, PropertyBooking>().ReverseMap();
    }
}