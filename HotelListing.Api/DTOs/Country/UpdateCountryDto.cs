namespace HotelListing.Api.DTOs.Country;

public class UpdateCountryDto : CreateCountryDto
{
    public required int CountryId { get; set; }
}