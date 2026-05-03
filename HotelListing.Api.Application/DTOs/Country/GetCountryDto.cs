using HotelListing.Api.Application.DTOs.Hotel;

namespace HotelListing.Api.Application.DTOs.Country;

public record GetCountryDto(
    int CountryId,
    string Name,
    string ShortName,
    List<GetHotelSlimDto> Hotels
);