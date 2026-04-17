using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;

namespace HotelListing.Api.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountriesDto>();
        CreateMap<Country, GetCountryDto>();
        CreateMap<CreateCountryDto, Country>();
    }
}