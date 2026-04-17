using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.MappingProfiles;

public class HotelMappingProfile :  Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, GetHotelsDto>()
            .ForMember(
                dest => dest.CountryName,
                opt => opt.MapFrom<CountryNameResolver>()
            );

        CreateMap<CreateHotelDto, Hotel>();
    }
}

public class CountryNameResolver : IValueResolver<Hotel, GetHotelsDto, string>
{
    public string Resolve(Hotel source, GetHotelsDto destination, string destMember, ResolutionContext context)
    {
        return source.Country?.Name ??  string.Empty;
    }
}