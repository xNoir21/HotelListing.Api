using AutoMapper;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Domain;

namespace HotelListing.Api.Application.MappingProfiles;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, GetHotelsDto>()
            .ForMember(
                dest => dest.CountryName,
                opt => opt.MapFrom<CountryNameResolver>()
            );

        CreateMap<CreateHotelDto, Hotel>();
        CreateMap<Hotel, GetHotelSlimDto>();
    }
}

public class CountryNameResolver : IValueResolver<Hotel, GetHotelsDto, string>
{
    public string Resolve(Hotel source, GetHotelsDto destination, string destMember, ResolutionContext context)
    {
        return source.Country?.Name ?? string.Empty;
    }
}