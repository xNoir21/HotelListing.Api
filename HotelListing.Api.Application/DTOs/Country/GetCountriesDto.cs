namespace HotelListing.Api.Application.DTOs.Country;

public record GetCountriesDto(
    int CountryId,
    string Name,
    string ShortName
);