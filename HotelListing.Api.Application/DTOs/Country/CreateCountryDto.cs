using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Application.DTOs.Country;

public class CreateCountryDto
{
    private const int NameMaxLength = 50;
    private const int ShortNameMaxLength = 3;

    [Required] [MaxLength(NameMaxLength)] public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(ShortNameMaxLength)]
    public string ShortName { get; set; } = string.Empty;
}