using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Country;

public class CreateCountryDto
{
    [MaxLength(50)]
    public required string Name { get; set; }
    [MaxLength(3)]
    public required string ShortName { get; set; }
}