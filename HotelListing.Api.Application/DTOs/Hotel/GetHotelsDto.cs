namespace HotelListing.Api.Application.DTOs.Hotel;

public record GetHotelsDto(
    int Id,
    string Name,
    string Address,
    double Rating,
    string CountryName,
    int CountryId
);